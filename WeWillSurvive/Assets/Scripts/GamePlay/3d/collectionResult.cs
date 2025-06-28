using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace WeWillSurvive
{
    public class collectionResult : MonoBehaviour
    {
        public List<int> items = Enumerable.Repeat(0, 2).ToList(); // [0, 0]
        public List<int> crew = Enumerable.Repeat(0, 4).ToList();  // [0, 0, 0, 0]

        public void Add(List<int> addItems, List<int> addCrews)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i] += (i < addItems.Count) ? addItems[i] : 0;
            }

            for (int i = 0; i < crew.Count; i++)
            {
                crew[i] += (i < addCrews.Count) ? addCrews[i] : 0;
            }
        }


        public void Escape()
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!you have escaped!!!!!!!!!!!!!!");
        }
    }
}
