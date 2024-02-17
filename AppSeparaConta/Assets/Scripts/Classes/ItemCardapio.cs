using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCardapio
{
    public string Id;
    public string Nome;
    public double PrecoTotal;

    public ItemCardapio()
    {
        Id = Guid.NewGuid().ToString();
    }

    public ItemCardapio(string _nome, double _preco)
    {
        Id = Guid.NewGuid().ToString();
        Nome = _nome;
        PrecoTotal = _preco;
    }

    public ItemCardapio(string _id, string _nome, double _preco)
    {
        Id= _id;
        Nome = _nome;
        PrecoTotal= _preco;
    }

}
