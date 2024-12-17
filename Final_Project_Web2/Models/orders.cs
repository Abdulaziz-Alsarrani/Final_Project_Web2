
using Microsoft.AspNetCore.Mvc; // For [BindProperty]
using System.ComponentModel.DataAnnotations; // For [DataType]

namespace Final_Project_Web2.Models
{
    public class orders
    {
        public int Id { get; set; }
        public string custname { get; set; }

        [BindProperty, DataType(DataType.Date)]
        public DateTime orderdate { get; set; }
        public int total { get; set; }
    }
}
