using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityIdHandler
{
    private static EntityIdHandler instance;

    public static EntityIdHandler GetInstance()
    {
        if (instance == null)
        {
            instance = new EntityIdHandler();
        }

        return instance;
    }

    private int id = 0;

    public int GetId()
    {
        id++;
        return id;
    }
}
