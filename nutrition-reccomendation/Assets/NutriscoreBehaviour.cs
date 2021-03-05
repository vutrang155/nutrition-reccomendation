using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using TMPro; // Text Mesh Pro

namespace ProductFactory 
{
    public class Product
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public List<string> Categories { get; set; }
        public string NutriScore { get; set; }
        public string NutriScore_100g { get; set; }
        public int KCal_100g { get; set; }

        public List<Product> OtherProducts { get; set; } = new List<Product>();

        public Product()
        {
            Categories = new List<string>();
        }
        public Product(string ID_entered)
        {
            ID = ID_entered;
            Categories = new List<string>();
        }

        public Product(string id, string name, string brand, List<string> categories, string nutriscore, string nutriscore_100, int kcal)
        {
            this.ID = id;
            this.Name = name;
            this.Brand = brand;
            this.Categories = categories;
            this.NutriScore = nutriscore;
            this.NutriScore_100g = nutriscore_100;
            this.KCal_100g = kcal;
        }

        public void searchProduct()
        {
            string filePath = Application.streamingAssetsPath + "/nutritionDatabase_final.json";
            try
            {
                //peut etre besoin de changer le chemin en fonction de votre config
                using (StreamReader r = new StreamReader(filePath))
                {
                    //TextAsset t = Resources.Load("nutritionDatabase.json") as TextAsset;
                    //string json = t.text;
                    string json = r.ReadToEnd();
                    dynamic array = JsonConvert.DeserializeObject(json);
                    bool found = false;

                    foreach (var product in array["data"])
                    {
                        if ((string) product["id"] == ID)
                        {
                            found = true;
                            Name = (string) product["product_name"];
                            Brand = (string) product["brands"];
                            foreach (string cat in product["main_category_fr"])
                            {
                                Categories.Add(cat);
                            }
                            NutriScore = (string) product["nutrition_grade_fr"];
                            NutriScore_100g = (string) product["nutrition_score_fr_100g"];
                            KCal_100g = (int) product["energy_kcal_100g"];
                        }
                        else {
                            string _ID = (string) product["id"];
                            string _Name = (string) product["product_name"];
                            string _Brands = (string) product["brands"];
                            List<string> _list = new List<string>();
                            foreach (string c in product["main_category_fr"])
                            {
                                _list.Add(c);
                            }
                            string _Nutrition_grade_fr = (string) product["nutrition_grade_fr"];
                            string _Nutrition_score_fr_100g = (string) product["nutrition_score_fr_100g"];
                            int _Energy_kcal_100g = (int) product["energy_kcal_100g"];

                            Product p = new Product(_ID, _Name, _Brands, _list, _Nutrition_grade_fr, _Nutrition_score_fr_100g, _Energy_kcal_100g);
                            OtherProducts.Add(p);
                        }
                    }

                    if (!found) throw new Exception("ProductID " + ID + " not found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e); 
            }
        }
    }

    public class Factory
    {
        public Product chargeProduct(string ID_detected)
        {
            Product product = new Product(ID_detected);
            product.searchProduct();
            return product;
        }
        
        public List<Product> sortList(List<Product> l)
        {
            var sortedList = l.OrderBy(si => si.KCal_100g).ToList();
            var sortedListBeta = sortedList.OrderBy(si => si.NutriScore).ToList();
            return sortedListBeta;
        }

         //sort a DICTIONARY by nutriscore & energy 
        public Dictionary<Product, int> productSortDictionary_nutriscoreANDenergy(Dictionary<Product, int> l)
        {
            Dictionary<Product, int> sortedDict = new Dictionary<Product, int>();
            foreach(var item in l.OrderBy(key => key.Key.KCal_100g)){
                sortedDict.Add(item.Key, item.Value);
            }

            Dictionary<Product, int> sortedDict_beta = new Dictionary<Product, int>();
            foreach (var item in sortedDict.OrderBy(key => key.Key.NutriScore))
            {
                sortedDict_beta.Add(item.Key, item.Value);
            }

            return sortedDict_beta;
        }

        
        //sort a DICTIONARY by categories 
        public Dictionary<Product, int> productSort_categories(Dictionary<Product, int> l)
        {
            Dictionary<Product, int> sortedDict = new Dictionary<Product, int>();

            foreach (var item in l.OrderByDescending(key => key.Value))
            {
                sortedDict.Add(item.Key, item.Value);
            }

            return sortedDict;
        }
    }



    // Main de test, peut etre supprimé s'il n'est pas nécéssaire
    /*
        class Program
        {
            static void Main(string[] args)
            {
                Factory f = new Factory();
                Product p = f.chargeProduct("3021769505619");
                Console.WriteLine("NutriScore " + p.nutriScore);
            }
        }
    */
}

public class NutriscoreBehaviour : MonoBehaviour
{
    public string ProductID;
    public TextMeshPro NutriscoreValue;
    public TextMeshPro KCalValue;
    public GameObject Emoji;

    // Start is called before the first frame update
    void Start()
    {
        // Hide object :
        this.gameObject.SetActive(true);
        
        if (ProductID == "") ProductID = "5411188100835";


        // Load ProductID
        ProductFactory.Factory f = new ProductFactory.Factory();
        ProductFactory.Product p = f.chargeProduct(ProductID);
        // TODO : make Product ID
        // ProductFactory.Product p = f.chargeProduct(ProductID);

        // Change KCalValue and NutriscoreValue 
        NutriscoreValue.text = p.NutriScore;
        KCalValue.text = p.KCal_100g.ToString();

        // Change Emoji
        if (NutriscoreValue.text == "A" || NutriscoreValue.text == "B") {
            Material good = Resources.Load("Materials/good", typeof(Material)) as Material;
            Emoji.GetComponent<MeshRenderer>().material = good; 
        }
        else if (NutriscoreValue.text == "C") {
            Material meh = Resources.Load("Materials/meh", typeof(Material)) as Material;
            Emoji.GetComponent<MeshRenderer>().material = meh; 
        }
        else {
            Material bad = Resources.Load("Materials/bad", typeof(Material)) as Material;
            Emoji.GetComponent<MeshRenderer>().material = bad; 
        }
    }
    // Update is called once per frame
    void Update()
    {
    }
}
