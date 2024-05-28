using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Technical Machine", menuName = "My Assets/New TM")]
public class TM : ScriptableObject
{
    public Sprite sprite;
    public Move move;

    public bool TeachMove(int target)
    {
        /*
        InventoryManager im = GameManager.instance.im;
        if (im.ownedCritters[target].moveset.Count > 3)
            return false;
        if (MovesetContain(target, im))
            return false;
        for (int i = 0; i <= im.ownedCritters[target].critter.stats.movesByMachine.Count - 1; i++)
        {
            List<TM> cachedList = im.ownedCritters[target].critter.stats.movesByMachine;
            if (cachedList[i] == this)
            {
                LearnedMove newMove = new LearnedMove();
                newMove.move = move;
                newMove.timesUsed = 0;

                return true;
            }
        }
        */
        return false;
    }
    
    /*
    bool MovesetContain(int target, InventoryManager im)
    {
        for (int i = 0; i <= im.ownedCritters[target].moveset.Count - 1; i++)
        {
            if (im.ownedCritters[target].moveset[i].move == move)
                return true;
        }
        return false;
    }
    */
}
