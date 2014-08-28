using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Media;
using DrWPF.Windows.Data;
using KyleHughes.CIS2118.KPUSim.Assembly;
using KyleHughes.CIS2118.KPUSim.Exceptions;
using KyleHughes.CIS2118.KPUSim.Views;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Input;
using System.ComponentModel;
using KyleHughes.CIS2118.KPUSim.Peripherals;
using System.Reflection;
using Newtonsoft.Json;

namespace KyleHughes.CIS2118.KPUSim.ViewModels
{
    /// <summary>
    /// current state of the program
    /// </summary>
    public enum RunState
    {
        Compiled,
        Compiling,
        Ready,
        Running,
        RunError,
        CompileError
    }
    /// <summary>
    /// the main view model!
    /// </summary>
    public partial class MainViewModel : NotifiableBase
    {
        /// <summary>
        /// internal example viewmodel
        /// </summary>
        public ExamplesViewModel ExamplesViewModel { get; set; }
        /// <summary>
        /// internal peripheral viewmodel
        /// </summary>
        public PeripheralsViewModel PeripheralsViewModel { get; set; }
        /// <summary>
        /// for binding in the about dialog (go check!)
        /// </summary>
        public string ExecutableVersion
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        /// <summary>
        /// current peripherals
        /// </summary>
        public ObservableDictionary<ushort, PeripheralBase> AttachedPeripherals { get; set; }

        private static MainViewModel _instance;
        private string _codeText;
        private RunState _runningState = RunState.Ready;
        private string _lastCompiledCode = "";
        private string _lastSavedCode = "";

        private MainViewModel()
        {
            //set loooooots of deafault values
            Speed = 500;
            ResetOnRun = true;
            IsProgramRunning = false;
            _codeRunner.WorkerSupportsCancellation = true;
            _codeRunner.DoWork += RunProgramBody;
            Stack = new ObservableStack<ushort>();
            Ram = new ObservableSortedDictionary<ushort, ushort>(new KeyComparer());
            Instructions = new ObservableSortedDictionary<ushort, IDisassemblyValue>(new KeyComparer());
            LabelMap = new ObservableDictionary<string, ushort>();
            this.AttachedPeripherals = new ObservableDictionary<ushort, PeripheralBase>();
            AssembleProgramCommand = new ActionCommand(BuildProgram, () => !IsProgramRunning);
            UnsatisfiedLabels = new List<string>();
            RunProgramCommand = new ActionCommand(RunProgram, () => RunningState == RunState.Compiled);
            StopProgramCommand = new ActionCommand(() =>
            {
                IsProgramRunning = false;
                _codeRunner.CancelAsync();
                RunningState = RunState.Compiled;
                Notify("IsProgramRunning");
                StopProgramCommand.NotifyCanExecute();
                AssembleProgramCommand.NotifyCanExecute();
            }, () => IsProgramRunning);
            SaveBinaryCommand = new ActionCommand(this.SaveBinary);
            OpenBinaryCommand = new ActionCommand(this.OpenBinary);
            SaveTextCommand = new ActionCommand(this.SaveText);
            OpenTextCommand = new ActionCommand(this.OpenText);
            ViewExamplesCommand = new ActionCommand(() => this.ExamplesViewModel.Start());
            CloseProgramCommand = new ActionCommand(() => Application.Current.Shutdown());
            ExamplesViewModel = new ExamplesViewModel();
            DetatchPeripheralCommand = new ParameteredActionCommand((peripheral) =>
            {
                ((KeyValuePair<ushort, PeripheralBase>)peripheral).Value.CleanUp();
                AttachedPeripherals.Remove(((KeyValuePair<ushort, PeripheralBase>)peripheral).Key);
            });
            this.PeripheralsViewModel = new PeripheralsViewModel();
            this.ViewInstructionsCommand = new ActionCommand(() => new InstructionsView().ShowDialog());
            this.ShowAboutDialogCommand = new ActionCommand(() => new AboutView().ShowDialog());
            this.ViewPeripheralsCommand = new ActionCommand(() => this.PeripheralsViewModel.Start());

            this.NewCommand = new ActionCommand(() =>
            {
                Stack.Clear();
                Registers.Reset();
                this.Instructions.Clear();
                Ram.Clear();
                LabelMap.Clear();
                foreach (KeyValuePair<ushort, PeripheralBase> val in AttachedPeripherals)
                {
                    val.Value.CleanUp();
                }
                AttachedPeripherals.Clear();
                this.CodeText = "";
            });
        }
        /// <summary>
        /// the current error (if there is one)
        /// </summary>
        public Exception ProgramException { get; set; }
        /// <summary>
        /// the text displayed at the bottom
        /// </summary>
        public string StateText
        {
            get
            {
                switch (RunningState)
                {
                    case RunState.Compiled:
                        return "Sucessful build";
                    case RunState.Compiling:
                        return "Building...";
                    case RunState.Ready:
                        return "Ready";
                    case RunState.Running:
                        return "Running Program...";

                    // TODO: Show errors
                    case RunState.RunError:
                    case RunState.CompileError:
                        return ProgramException.Message;
                }
                return "ERRORS";
            }
        }
        /// <summary>
        /// the colour of the bottom bar
        /// </summary>
        public SolidColorBrush StateColour
        {
            get
            {
                switch (RunningState)
                {
                    case RunState.Compiled:
                    case RunState.Compiling:
                        return new SolidColorBrush(Color.FromRgb(34, 177, 76));
                    case RunState.Ready:
                        return new SolidColorBrush(Color.FromRgb(0, 122, 204));
                    case RunState.Running:
                        return new SolidColorBrush(Color.FromRgb(202, 81, 0));
                    case RunState.RunError:
                    case RunState.CompileError:
                        return new SolidColorBrush(Color.FromRgb(136, 0, 21));
                }
                return Brushes.Black;
            }
        }
        /// <summary>
        /// the current state of the program
        /// </summary>
        public RunState RunningState
        {
            get { return _runningState; }
            set
            {
                _runningState = value;
                if (!value.IsError())
                    ProgramException = null;
                Notify("RunningState");
                Notify("StateColour");
                Notify("StateText");
                RunProgramCommand.NotifyCanExecute();
            }
        }
        /// <summary>
        /// singleton instance of this
        /// </summary>
        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MainViewModel();
                return _instance;
            }
        }
        //lots of commands
        public ActionCommand CloseProgramCommand { get; private set; }
        public ActionCommand SaveBinaryCommand { get; private set; }
        public ActionCommand OpenBinaryCommand { get; private set; }
        public ActionCommand SaveTextCommand { get; private set; }
        public ActionCommand OpenTextCommand { get; private set; }
        public ActionCommand ViewExamplesCommand { get; private set; }
        public ActionCommand ViewInstructionsCommand { get; private set; }
        public ActionCommand ShowAboutDialogCommand { get; private set; }
        public ActionCommand ViewPeripheralsCommand { get; private set; }
        public ActionCommand NewCommand { get; private set; }
        public ParameteredActionCommand DetatchPeripheralCommand { get; private set; }


        /// <summary>
        /// speed the program runs at
        /// </summary>
        public int Speed { get; set; }
        /// <summary>
        /// source code
        /// </summary>
        public string CodeText
        {
            get { return _codeText ?? ""; }
            set
            {
                _codeText = value;
                Notify("HasCodeChanged");
                Notify(null);
            }
        }
        /// <summary>
        /// throw a program exception
        /// </summary>
        /// <param name="e">exception to throw</param>
        public void ThrowException(ProgramException e)
        {
            IsProgramRunning = false;
            if (_codeRunner.IsBusy)
            {
                _codeRunner.CancelAsync();
            }
            ProgramException = e;
            if (e.IsCompileError)
                RunningState = RunState.CompileError;
            RunningState = RunState.RunError;
        }
        /// <summary>
        /// saves a .kpub
        /// </summary>
        private void SaveBinary()
        {
            //got a nice warning dialog
            if ((bool)App.AppSettings["ShowBinaryWarning"])
            {
                new OpenBinaryWarningDialog().ShowDialog();
            }
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".kpub",
                Filter = "KPU binary files (.kpub)|*.kpub",
                AddExtension = true,
                Title = "Save KPU Binary File"
            };

            bool? ret = dialog.ShowDialog();
            if (ret != true)
                return;

            //write to the file
            using (var stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (ushort val in Ram.Values)
                    {
                        writer.Write(val);
                    }
                }
            }
        }
        /// <summary>
        /// opens a .kpub
        /// </summary>
        private void OpenBinary()
        {

            //just as above but backwards
            if ((bool)App.AppSettings["ShowBinaryWarning"])
            {
                new OpenBinaryWarningDialog().ShowDialog();
            }
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".kpub",
                Filter = "KPU binary files (.kpub)|*.kpub",
                AddExtension = true,
                Title = "Open KPU Binary File"
            };

            bool? ret = dialog.ShowDialog();

            if (ret != true)
                return;
            try
            {
                using (var stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ushort i = 0;
                        Ram = new ObservableSortedDictionary<ushort, ushort>(new KeyComparer());
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            Ram.Add(i, reader.ReadUInt16());
                            i++;
                        }
                    }
                }
                this.DisassembleProgram();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("File not found", "Oops!", MessageBoxButton.OK, MessageBoxImage.Information,
                    MessageBoxResult.OK);
            }
            Notify(null);
        }
        /// <summary>
        /// opens a .kpu
        /// </summary>
        private void OpenText()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = ".kpu",
                Filter = "KPU program files (.kpu)|*.kpu",
                AddExtension = true,
                Title = "Open KPU program File"
            };

            bool? ret = dialog.ShowDialog();
            if (ret != true)
                return;
            string json = File.ReadAllText(dialog.FileName); //deserialise the json
            SaveContainer c = JsonConvert.DeserializeObject<SaveContainer>(json, new JsonSerializerSettings()
            {
                TypeNameHandling=TypeNameHandling.All
            });
            this.AttachedPeripherals = c.UsedPeripherals;
            this.CodeText = c.Code; //set the stuffs
        }
        /// <summary>
        /// whether the code has changed since last being saved
        /// </summary>
        public bool IsUnsaved
        {
            get
            {
                return _lastSavedCode != CodeText;
            }
        }
        /// <summary>
        /// saves a .kpu
        /// </summary>
        private void SaveText()
        {
            //as above but backwards
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".kpu",
                Filter = "KPU program files (.kpu)|*.kpu",
                AddExtension = true,
                Title = "Save KPU program File"
            };

            bool? ret = dialog.ShowDialog();
            if (ret != true)
                return;


            SaveContainer container = new SaveContainer()
            {
                Code = CodeText,
                UsedPeripherals = AttachedPeripherals
            };
            //serialise the object
            File.WriteAllText(dialog.FileName, JsonConvert.SerializeObject(container, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            }));
            _lastSavedCode = CodeText;
            Notify("IsUnsaved");
        }
    }
}