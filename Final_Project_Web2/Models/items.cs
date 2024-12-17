﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc; // For [BindProperty]
using System.ComponentModel.DataAnnotations; // For [DataType]


namespace Final_Project_Web2.Models
{
    public class items
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public string discount { get; set; }
        [BindProperty, DataType(DataType.Date)]

        public DateTime makedate { get; set; }
        public int category { get; set; }
        public int quantity { get; set; }
        public string imgfile { get; set; }
    }
}