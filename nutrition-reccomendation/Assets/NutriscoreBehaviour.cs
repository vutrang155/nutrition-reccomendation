using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

        public Product()
        {
            Categories = new List<string>();
        }
        public Product(string ID_entered)
        {
            ID = ID_entered;
            Categories = new List<string>();
        }

        public void searchProduct()
        {
            string filePath = Application.streamingAssetsPath + "/nutritionDatabase.json";
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
                    }

                    if (!found) throw new Exception("ProductID not found");
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
        // Load ProductID
        ProductFactory.Factory f = new ProductFactory.Factory();
        ProductFactory.Product p = f.chargeProduct("5053827203760");
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
