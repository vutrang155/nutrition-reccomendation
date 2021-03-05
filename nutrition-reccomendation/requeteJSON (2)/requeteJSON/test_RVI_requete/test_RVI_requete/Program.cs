using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace System.Windows { 
    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string brands { get; set; }
        public List<string> main_category_fr { get; set; }
        public string nutrition_grade_fr { get; set; }
        public string nutrition_score_fr_100g { get; set; }
        public int energy_kcal_100g { get; set; }

        public List<Product> products { get; set; } = new List<Product>();


        public Product()
        {
            main_category_fr = new List<string>();
        }

        public Product(string ID_entered)
        {
            id = ID_entered;
            main_category_fr = new List<string>();
        }

        public Product(string ID, string NAME, string BRANDS, List<string> categories, string nutriscore, string nutriscore_100, int kcal)
        {
            id = ID;
            name = NAME;
            brands = BRANDS;
            main_category_fr = new List<string>();
            main_category_fr = categories;
            nutrition_grade_fr = nutriscore;
            nutrition_score_fr_100g = nutriscore_100;
            energy_kcal_100g = kcal;
        }

        public void myProduct()
        {

            using (StreamReader r = new StreamReader("/Users/emiliabenelguemar/Desktop/Polytech/ET5/RVI/TP/requeteJSON/nutritionDatabase.json"))
            {
                string json = r.ReadToEnd();
                dynamic array = JsonConvert.DeserializeObject(json);


                foreach (var product in array.data)
                {
                    if (product.id == id)    //load the right product
                    {
                        name = product.product_name;
                        brands = product.brands;
                        foreach (string c in product.main_category_fr)
                        {
                            main_category_fr.Add(c);
                        }
                        nutrition_grade_fr = product.nutrition_grade_fr;
                        nutrition_score_fr_100g = product.nutrition_score_fr_100g;
                        energy_kcal_100g = product.energy_kcal_100g;
                    }
                    else    //add other product in list Products
                    {
                        string ID = product.id;
                        string Name = product.product_name;
                        string Brands = product.brands;
                        List<string> list = new List<string>();
                        foreach (string c in product.main_category_fr)
                        {
                            list.Add(c);
                        }
                        string Nutrition_grade_fr = product.nutrition_grade_fr;
                        string Nutrition_score_fr_100g = product.nutrition_score_fr_100g;
                        int Energy_kcal_100g = product.energy_kcal_100g;

                        Product p = new Product(ID, Name, Brands, list, Nutrition_grade_fr, Nutrition_score_fr_100g, Energy_kcal_100g);
                        products.Add(p);
                    }
                }
            }
        }
    }

    public class Factory
    {
        public Product chargeProduct(string ID_detected)
        {
           Product product = new Product(ID_detected);
           product.myProduct();
            return product;
        }

        public List<Product> productSort_nutriscoreANDenergy(List<Product> l)
        {
            var sortedList = l.OrderBy(si => si.energy_kcal_100g).ToList();
            var sortedList_beta = sortedList.OrderBy(si => si.nutrition_grade_fr).ToList();
            return sortedList_beta;
        }
    }




    class Program
    {
        static void Main(string[] args)
        {
            //read product
            Factory f = new Factory();
            Console.WriteLine("test charge product");
            Product p = f.chargeProduct("5411188110842");
            Console.WriteLine("Name product : " + p.name);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            /*********** SIMILAR PRODUCTS ***********/

            Dictionary<Product, int> CommonProduct2 = new Dictionary<Product, int>();

            foreach(Product pp in p.products)
            {
                List<string> List1 = new List<string>();
                    List1 = pp.main_category_fr;
                List<string> List2 = new List<string>();
                    List2 = p.main_category_fr;

                string[] Result = List1.Intersect(List2).ToArray(); //return a list of the same elements in 2 lists

                if(Result.Count() > 1) // 0 : list not empty | 1 : or list of at least 2 elements
                {
                    CommonProduct2.Add(pp, Result.Count());
                }

            }

            //avant tri categorie
            Console.WriteLine("TEST BEFORE TRI CATEGORIES");
            foreach (KeyValuePair<Product, int> de in CommonProduct2)
            {
                Console.WriteLine("    {0}  {1}  {2}  ", de.Key.name, de.Key.energy_kcal_100g, de.Value);
            }
            //après tri categorie
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("TEST AFTER TRI CATEGORIES");
            List<Product> myList = new List<Product>();
            foreach (var item in CommonProduct2.OrderByDescending(key => key.Value)){   //sort list in descending order of common categories with the initial product
                Console.WriteLine("   {0}  {1}  {2}  ", item.Key.name, item.Key.energy_kcal_100g, item.Value);

                myList.Add(item.Key);


            }
            //CommonProduct2.OrderByDescending(key => key.Value);


            //trier selon nutriscore et kcal
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("TEST AFTER TRI CATEGORIES + ENERGY + KCAL");
            myList = f.productSort_nutriscoreANDenergy(myList);
            foreach(Product item in myList)
            {
                Console.WriteLine("   {0}  {1}  ", item.name, item.energy_kcal_100g);
            }


        }

    }

}
