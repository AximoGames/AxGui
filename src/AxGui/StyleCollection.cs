// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AxGui
{

    public class StyleCollection
    {

        public List<StyleRule> Rules = new List<StyleRule>();

        public static StyleCollection FromFile(string path)
        {
            var parser = new ExCSS.StylesheetParser();
            using var fs = File.OpenRead(path);
            var sheet = parser.Parse(fs);

            var col = new StyleCollection();
            foreach (ExCSS.StyleRule rule in sheet.StyleRules)
            {
                var style = new ElementStyle();
                ElementStyle.SetRule(style, rule.Style);
                col.Rules.Add(new StyleRule(rule.SelectorText, style));
            }
            return col;
        }

        public StyleRule? GetRuleByClass(string cssClass)
        {
            return GetRuleBySelector("." + cssClass);
        }

        public StyleRule? GetRuleBySelector(string selector) => Rules.LastOrDefault(x => x.Selector == selector);

    }

}
