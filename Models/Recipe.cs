using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Culinaria.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RecipeId { get; set; }
    }

    public class Recipe
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
        public int Quantity { get; set; }
        public string Prep { get; set; }
        public string Image { get; set; }
        public Ingredient[] Ingredients { get; set; }
    }
}
