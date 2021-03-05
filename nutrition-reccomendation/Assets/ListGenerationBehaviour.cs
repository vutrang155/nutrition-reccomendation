using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using TMPro; // Text Mesh Pro
public class ListGenerationBehaviour : MonoBehaviour
{
    public GameObject ListElementPrefab;
    
    private List<GameObject> ListItems = new List<GameObject>(); 
    // Start is called before the first frame update
    void Start()
    {
        transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void notify(TrackingHandler m, bool visibility) {
        transform.gameObject.SetActive(visibility);
        // On Detect
        string productId = m.ProductID;
        if (productId != null && productId != "")
        {
            if (visibility)
            {
                drawSuggestion(productId);
            }
            // On lost
            else
            {
                destroyListItems();
            }
        }
    }

    private void drawSuggestion(string productId) {
        if (productId == "") productId = "Fissure";

        // ############################################################################
        // ####################### CHARGER LES SUGGESTIONS ***#########################
        // Load ProductID
        ProductFactory.Factory f = new ProductFactory.Factory();
        ProductFactory.Product p = f.chargeProduct(productId);
        // Similar Product : 
        Dictionary<ProductFactory.Product, int> commonProduct = new Dictionary<ProductFactory.Product, int>();
        foreach(ProductFactory.Product pp in p.OtherProducts) {
            List<string> l1 = pp.Categories;
            List<string> l2 = p.Categories;
            string[] res = l1.Intersect(l2).ToArray();

            /*
            if (res.Count() > 1) {
                commonProduct.Add(pp, res.Count());
            }
            */
            if((res.Count() > 1) && (p.NutriScore.CompareTo(pp.NutriScore) >= 0)) // 0 : list not empty | 1 : or list of at least 2 elements
            {
                commonProduct.Add(pp, res.Count());
            }
        }

        List<ProductFactory.Product> myList = new List<ProductFactory.Product>();
        foreach (KeyValuePair<ProductFactory.Product, int> de in f.productSort_categories(f.productSortDictionary_nutriscoreANDenergy(commonProduct)))
        {
            myList.Add(de.Key);
        }
        // ############################################################################

        float currentPosY = transform.position.y/2;
        foreach (var item in myList) {
            // Initiate list item
            var listElem = Instantiate(ListElementPrefab) as GameObject;
            listElem.transform.parent = transform;
            listElem.transform.localPosition = new Vector3(0, currentPosY, 0);

            // 0 : BrandLabel
            // 1 : NameLabel
            // 2 : NutriscoreLabel
            // 3 : KcalValueLabel
            // TODO : Change labels contents
            var nameLabel = listElem.transform.GetChild(0).gameObject;
            nameLabel.GetComponent<Text>().text = item.Name;

            var brandLabel = listElem.transform.GetChild(1).gameObject;
            brandLabel.GetComponent<Text>().text = item.Brand;

            var nutriLabel = listElem.transform.GetChild(2).gameObject;
            nutriLabel.GetComponent<Text>().text = item.NutriScore_100g;

            var kcalLabel = listElem.transform.GetChild(3).gameObject;
            kcalLabel.GetComponent<Text>().text = item.KCal_100g.ToString();
            
            this.ListItems.Add(listElem);
            // Update current Pos 
            currentPosY = currentPosY - 105;

        }
    }

    private void destroyListItems() {
        foreach (var item in ListItems) {
            Destroy(item);
        }
        ListItems = new List<GameObject>();
    }
}
