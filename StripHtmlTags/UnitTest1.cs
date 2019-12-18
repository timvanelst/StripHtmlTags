using System;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StripHtmlTags
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void StripTags_removes_tags()
        {
            var html = "<html><body><div><span><h1>Some text with <b><u>formatting</u></b></h1></span></div></body></html>";
            var stripped = "<html><body><div><span>Some text with <b>formatting</b></span></div></body></html>";

            var htmlHelper = new HtmlHelper();
            var actualResult = htmlHelper.StripTags(html, "h1", "u");

            Assert.AreEqual(expected: stripped, actualResult);

        }

        [TestMethod]
        public void StripTags_removes_form_tag_but_leaves_the_rest_of_the_html_intact()
        {
            var html = @"<!doctype html>

<html lang=""en"">
<head>
  <meta charset=""utf-8"">

  <title>The HTML5 Herald</title>
  <meta name=""description"" content=""The HTML5 Herald"">
  <meta name=""author"" content=""SitePoint"">

  <link rel=""stylesheet"" href=""css/styles.css?v=1.0"">

</head>

<body>
    <p>
        Some text with <b>formatting</b>.
    </p>
    <form>

    </form>
    <script>
        alert('Test');
    </script>
</body>
</html>";
            var stripped = @"<!doctype html>

<html lang=""en"">
<head>
  <meta charset=""utf-8"">

  <title>The HTML5 Herald</title>
  <meta name=""description"" content=""The HTML5 Herald"">
  <meta name=""author"" content=""SitePoint"">

  <link rel=""stylesheet"" href=""css/styles.css?v=1.0"">

</head>

<body>
    <p>
        Some text with <b>formatting</b>.
    </p>
    

    
    
        alert('Test');
    
</body>
</html>";

            var htmlHelper = new HtmlHelper();
            var actualResult = htmlHelper.StripTags(html, "form", "script");

            Assert.AreEqual(expected: stripped, actualResult);

        }

        [TestMethod]
        public void StripTags_returns_original_when_it_contains_none_of_the_tags_to_strip()
        {
            var html = "<p>Some text</p>";

            var htmlHelper = new HtmlHelper();
            var actualResult = htmlHelper.StripTags(html, "h1", "b");

            Assert.AreEqual(expected: html, actualResult);
        }
    }

    public class HtmlHelper
    {
        public string StripTags(string html, params string[] tagsToStrip)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            foreach (var tagToStrip in tagsToStrip)
            {
                var selectedNodes = doc.DocumentNode.SelectNodes($"//{tagToStrip}") ?? Enumerable.Empty<HtmlNode>();
                foreach (var item in selectedNodes)
                {
                    var parent = doc.DocumentNode.SelectSingleNode(item.ParentNode.XPath);
                    parent.RemoveChild(item, keepGrandChildren: true);
                }
            }

            return doc.DocumentNode.OuterHtml;
        }
    }

}
