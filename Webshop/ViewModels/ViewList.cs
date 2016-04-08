using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Webshop.Models;

namespace Webshop.ViewModels
{
    public class ViewList
    {
        public List<Matratt> AllProducts { get; set; }
        public List<Matratt> SelectedProducts { get; set; }
        public decimal TotalPrice { get; set; }
    }
}