using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// represents a type of word identifier
    /// </summary>
    public class WordKind
    {
        /// <summary>
        /// this word's font style
        /// </summary>
        public FontStyle FontStyle;
        /// <summary>
        /// this word's font weight
        /// </summary>
        public FontWeight FontWeight;
        /// <summary>
        /// this word's colour
        /// </summary>
        public Brush ForegroundColour;
        /// <summary>
        /// get matches
        /// </summary>
        public Func<string, IEnumerable<Match>> GetMatches;
        /// <summary>
        /// this word's name
        /// </summary>
        public String Name;
        /// <summary>
        /// the priority for matching with this word
        /// </summary>
        public int Priority;
        /// <summary>
        /// Constructs a new wordkind
        /// </summary>
        /// <param name="priority">priority for matching</param>
        /// <param name="func">the match condition</param>
        /// <param name="col">colour</param>
        /// <param name="weight">weight</param>
        /// <param name="style">style</param>
        /// <param name="name">words name</param>
        public WordKind(int priority, Func<string, IEnumerable<Match>> func, Brush col,
            FontWeight? weight = null, FontStyle? style = null, [CallerMemberName] string name = "UNKNOWN TYPE")
        {
            Name = name;
            GetMatches = func;
            Priority = priority;
            WordKinds.AllItems.Add(this);
            //sort the word identifiers
            WordKinds.AllItems.Sort((e1, e2) =>
            {
                if (e1.Priority > e2.Priority)
                    return -1;
                if (e1.Priority < e2.Priority)
                    return 1;
                return 0;
            });
            if (weight == null)
                weight = FontWeights.Normal;
            FontWeight = (FontWeight)weight;
            if (style == null)
                style = FontStyles.Normal;
            FontStyle = (FontStyle)style;
            ForegroundColour = col;
        }
        /// <summary>
        /// format given text
        /// </summary>
        /// <param name="text"></param>
        public void Format(FormattedText text)
        {
            //for every match, format!
            foreach (Match v in GetMatches(text.Text))
            {
                text.SetForegroundBrush(ForegroundColour, v.Index, v.Length);
                text.SetFontWeight(FontWeight, v.Index, v.Length);
                text.SetFontStyle(FontStyle, v.Index, v.Length);

            }
        }
        /// <summary>
        /// returns whether the text matches at all
        /// </summary>
        /// <param name="text">given text</param>
        /// <returns></returns>
        public bool DoesMatch(string text)
        {
            return GetMatches(text).Any();
        }
    }
    /// <summary>
    /// Container class for all wordkinds
    /// </summary>
    public static class WordKinds
    {
        /// <summary>
        /// stores all wordkinds
        /// </summary>
        public static List<WordKind> AllItems = new List<WordKind>();

        /// <summary>
        /// register type
        /// </summary>
        public static WordKind Register = new WordKind(10, text =>
        {
            //basically concatenates together the names of all registers 
            string registerMatch = "(";
            foreach (Register v in Registers.All)
                registerMatch += v.Name + "|";
            registerMatch = registerMatch.Substring(0, registerMatch.Length - 1) + ")";
            //searches for them! I LOVE ME SOME REGULAR EXPRESSIONS =D
            return Regex.Matches(text, String.Format(@"(?i)(?<=(^|\s)){0}(?=$|\s)", registerMatch)).OfType<Match>();
        }, Brushes.SlateGray);

        ///
        public static WordKind OpCode = new WordKind(10, text =>
        {
            string registerMatch = "(";
            foreach (OpCode v in OpCodes.All)
                registerMatch += v.Mnemonic + "|";
            registerMatch = registerMatch.Substring(0, registerMatch.Length - 1) + ")";
            return Regex.Matches(text, String.Format(@"(?i)(?<=(^|\s)){0}(?=$|\s)", registerMatch)).OfType<Match>();
        }, Brushes.DodgerBlue, FontWeights.Bold);

        public static WordKind MemoryLiteral = new WordKind(60,
            text =>
            {
                return ///i'm a big fan of this one. finds a literal hex or decimal or label value preceeded by a *
                    Regex.Matches(text, @"(?<=^|\s)\*((>[a-zA-Z_]+)|(0[xX][a-f0-9A-F]+)|([0-9]+))(?=\s|$)")
                        .OfType<Match>();
            }, Brushes.DarkViolet, FontWeights.Normal, FontStyles.Italic);

        /// <summary>
        /// i don't need to comment the rest i'm sure?
        /// </summary>
        public static WordKind MemoryRegister = new WordKind(60, text =>
        {
            string registerMatch = "(";
            foreach (Register v in Registers.All)
                registerMatch += v.Name + "|";
            registerMatch = registerMatch.Substring(0, registerMatch.Length - 1) + ")";
            return Regex.Matches(text, String.Format(@"(?i)(?<=^|\s)\*{0}(?=\s|$)", registerMatch)).OfType<Match>();
        }, Brushes.DarkViolet, FontWeights.Normal, FontStyles.Italic);

        public static WordKind HexadecimalLiteral = new WordKind(50,
            text => { return Regex.Matches(text, @"(?<=^|\s)0[xX][a-f0-9A-F]+(?=\s|$)").OfType<Match>(); },
            Brushes.Orange);

        public static WordKind DecimalLiteral = new WordKind(50,
            text => { return Regex.Matches(text, @"(?<=^|\s)[0-9]+(?=\s|$)").OfType<Match>(); }, Brushes.Orange);

        public static WordKind Comment = new WordKind(90,
            text => { return Regex.Matches(text, @"(?<=(^|\s))'([^']*)'(?=$|\s)").OfType<Match>(); }, Brushes.Green,
            FontWeights.Bold, FontStyles.Italic);

        public static WordKind StringLiteral = new WordKind(90,
            text => { return Regex.Matches(text, @"(?<=(^|\s))""([^""]*)""(?=$|\s)").OfType<Match>(); }, Brushes.Brown,
            FontWeights.Bold);

        public static WordKind LabelDeclaration = new WordKind(100,
            text => { return Regex.Matches(text, @"(?<=^|\s)\$[a-zA-Z_]+(?=\s|$)").OfType<Match>(); }, Brushes.DeepPink,
            FontWeights.Normal, FontStyles.Italic);

        public static WordKind LabelReference = new WordKind(60,
            text => { return Regex.Matches(text, @"(?<=^|\s)>[a-zA-Z_]+(?=\s|$)").OfType<Match>(); }, Brushes.HotPink,
            FontWeights.Normal, FontStyles.Italic);
    }
}