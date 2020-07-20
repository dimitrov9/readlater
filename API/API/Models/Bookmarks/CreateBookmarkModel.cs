using DataAnnotationsExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using UrlAttribute = DataAnnotationsExtensions.UrlAttribute;

namespace API.Models.Bookmarks
{
    public class CreateBookmarkModel
    {
        [Required, MaxLength(500)]
        [Url(UrlOptions.OptionalProtocol)]
        public string Url { get; set; }
        public string Description { get; set; }
        public int? CategoryId { get; set; }
    }
}