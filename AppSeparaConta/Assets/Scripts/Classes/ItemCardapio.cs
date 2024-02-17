using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemCardapio
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public double PrecoTotal { get; set; }

    public ItemCardapio()
    {

    }

    public ItemCardapio(string _nome, double _preco)
    {
        Id = Guid.NewGuid();
        Nome = _nome;
        PrecoTotal = _preco;
    }

    public ItemCardapio(Guid _id, string _nome, double _preco)
    {
        Id= _id;
        Nome = _nome;
        PrecoTotal= _preco;
    }

}
