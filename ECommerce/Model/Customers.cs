﻿using System.ComponentModel.DataAnnotations;

namespace ECommerce.Model
{
   
    public class Customers
    {
        
        public string? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? HouseNo { get; set; }
        public string? Street { get; set; }
        public string? Town { get; set; }
        public string? Postcode { get; set; }
    }
}
