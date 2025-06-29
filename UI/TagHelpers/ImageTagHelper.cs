using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;


namespace OSS.UI.TagHelpers
{
    [HtmlTargetElement("img", Attributes = "img-action,img-controller")]
    public class ImageTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;

        public string ImgController { get; set; }
        public string ImgAction { get; set; }

        public ImageTagHelper(LinkGenerator linkGenerator)
        {
            _linkGenerator = linkGenerator;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var src = _linkGenerator.GetPathByAction(ImgAction, ImgController);

            output.Attributes.SetAttribute("src", src);

            // Удаляем наши кастомные атрибуты
            output.Attributes.RemoveAll("img-action");
            output.Attributes.RemoveAll("img-controller");
        }
    }


}
