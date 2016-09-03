using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iep_ecommerce.Models.Tokens
{
    public enum OrderState { PROCESSING, SUCCESSFUL, FAILED };

    public class Order
    {    
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public ApplicationUser User { get; set; }

        public int NumberOfTokens { get; set; }

        public OrderState Status { get; set; }
    }
}