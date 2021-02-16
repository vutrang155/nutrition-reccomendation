using UnityEngine;
using UnityEngine.UI;
public class ListGenerationBehaviour : MonoBehaviour
{
    public GameObject ListElementPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        float currentPosY = transform.position.y/2;
        for(int i = 0; i < 10; i++) {
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
            nameLabel.GetComponent<Text>().text = "Hello";

            var brandLabel = listElem.transform.GetChild(1).gameObject;
            brandLabel.GetComponent<Text>().text = "HelloBrand";

            var nutriLabel = listElem.transform.GetChild(2).gameObject;
            nutriLabel.GetComponent<Text>().text = "HelloBrand";

            var kcalLabel = listElem.transform.GetChild(3).gameObject;
            kcalLabel.GetComponent<Text>().text = ""+i;
            
            currentPosY = currentPosY - 105;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
