// This file is part of AxGUI. Web: https://github.com/AximoGames
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AxGui
{

    public class StyleCollection
    {

        public List<StyleRule> Rules = new List<StyleRule>();

        public static StyleCollection FromString(string? content)
        {
            if (content == null)
                return new StyleCollection();

            using var fs = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return FromStream(fs);
        }

        public static StyleCollection FromFile(string path)
        {
            using var fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return FromStream(fs);
        }

        public static StyleCollection FromStream(Stream stream)
        {
            var parser = new ExCSS.StylesheetParser(includeUnknownDeclarations: true);
            var sheet = parser.Parse(stream);

            var col = new StyleCollection();
            foreach (ExCSS.StyleRule rule in sheet.StyleRules)
            {
                var style = new ElementStyle();
                ElementStyle.SetRule(style, rule.Style);
                col.Rules.Add(new StyleRule(rule.SelectorText, style));
            }
            return col;
        }

        public StyleRule? GetRuleByClass(string? cssClass)
        {
            if (string.IsNullOrEmpty(cssClass))
                return null;

            return GetRuleBySelector("." + cssClass);
        }

        public StyleRule? GetRuleById(string? id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            return GetRuleBySelector("#" + id);
        }

        public StyleRule? GetRuleByTag(string? cssClass)
        {
            if (string.IsNullOrEmpty(cssClass))
                return null;

            return GetRuleBySelector(cssClass);
        }

        public StyleRule? GetRuleBySelector(string? selector) => Rules.LastOrDefault(x => x.Selector == selector);

    }

}
