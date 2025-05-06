using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace RazorPagesIntro.Pages
{
    public class Formularz : PageModel
    {
        public string Message { get; private set; } = "Model strony stworzony w C#";

        public void OnGet()
        {
            if (Request.Query != null)
                Message += Request.Query["mojepole"].ToString();
        }

    }
}