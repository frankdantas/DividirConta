using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Controle : MonoBehaviour
{
    public TMP_InputField inputNomeRole;
    [Space]
    [Header("Cardapio")]
    public TMP_InputField inputNomeItemCardapio;
    public TMP_InputField inputPrecoItemCardapio;
    [Space]
    [Header("Lista consumo")]
    public TMP_Dropdown dropItensCardapio;
    public TMP_Text txtValorTotalItem;
    public TMP_Dropdown dropDivididoEm;
    public TMP_Dropdown dropPagarPartes;
    public TMP_Text txtSubValorItem;
    public TMP_Text txtConsumoTotal;
    [Space]
    public GameObject painelCadCardapio;
    [Space]
    public Transform painelContentLista;
    public GameObject prefabItemLista;


    private ConsumoTotal _consumoTotal = new ConsumoTotal();
    private List<ItemCardapio> cardapio = new List<ItemCardapio>();
    private ItemCardapio itemPreselecionado = new ItemCardapio();

    public static Controle SINGLETON = null;

    void Start()
    {
        if(SINGLETON == null)
        {
            SINGLETON = this;
        }

        PopulaDrops();
        AtualizarItensDropCardapio();
        onclick_fecharPainelCadCardapio();
    }
    
    void Update()
    {
        
    }

    private void PopulaDrops()
    {
        dropDivididoEm.ClearOptions();
        dropPagarPartes.ClearOptions();

        List<string> lista = new List<string>();
        for (int i = 1; i < 21; i++)
        {
            lista.Add(i.ToString());
        }
        dropDivididoEm.AddOptions(lista);
        onchange_mudouDivididoEm(0);
    }


    public void onclick_novoItemCardapio()
    {
        string nome = inputNomeItemCardapio.text.Trim();
        double preco = 0;
        if(double.TryParse(inputPrecoItemCardapio.text, out preco))
        {
            ItemCardapio novoItem = new ItemCardapio(nome, preco);
            cardapio.Add(novoItem);

            cardapio = cardapio.OrderBy(x => x.Nome).ToList();

            AtualizarItensDropCardapio();
            onclick_fecharPainelCadCardapio();
            inputNomeItemCardapio.text = "";
            inputPrecoItemCardapio.text = "";
        }


    }

    private void AtualizarItensDropCardapio()
    {
        dropItensCardapio.ClearOptions();
        List<string> itensDrop = cardapio.Select(x => x.Nome).ToList();
        itensDrop.Insert(0, "--- Selecione ---");
        dropItensCardapio.AddOptions(itensDrop);
    }

    public void onchange_mudouItemDropCardapio(int index)
    {
        ItemCardapio itemSelecionado = cardapio[index-1];
        if (itemSelecionado != null)
        {
            itemPreselecionado = itemSelecionado;
            AtualizaDadosItemTela();
        }
    }
    public void onchange_mudouDivididoEm(int index)
    {
        dropPagarPartes.options.Clear();
        List<string> lista = new List<string>();
        for (int i = 0; i < dropDivididoEm.value+1; i++)
        {
            lista.Add($"{i+1}/{dropDivididoEm.value+1}");
        }
        dropPagarPartes.AddOptions(lista);
        AtualizaDadosItemTela();
    }
    public void onchange_mudouPartesPagas(int index)
    {
        AtualizaDadosItemTela();
    }


    private ItemConsumo AtualizaDadosItemTela()
    {
        txtValorTotalItem.text = string.Format("{0:C2}", itemPreselecionado.PrecoTotal);
        ItemConsumo itemConsumo = new ItemConsumo() { DivididoEm = dropDivididoEm.value + 1, PartesPagas = dropPagarPartes.value + 1, ItemDoCardapio = itemPreselecionado };
        txtSubValorItem.text = string.Format("{0:C2}", itemConsumo.GetSubTotal());

        return itemConsumo;
    }

    public void onclick_adicionarConsumo()
    {
        ItemConsumo item = AtualizaDadosItemTela();
        _consumoTotal.ListaConsumo.Add(item);
        print("Adicionado " + item.ToString());

        GameObject copia = Instantiate(prefabItemLista, painelContentLista.position, Quaternion.identity, painelContentLista);
        copia.GetComponent<ItemConsumido>().SetValores(item);

        AtualizarConsumoTotal();
    }

    private void AtualizarConsumoTotal()
    {
        txtConsumoTotal.text = string.Format("Total: {0:C2}", _consumoTotal.GetConsumoTotal());
    }

    public void onclick_abrirPainelCadCardapio()
    {
        painelCadCardapio.SetActive(true);
    }

    public void onclick_fecharPainelCadCardapio()
    {
        painelCadCardapio.SetActive(false);
    }

    public void deletarItemConsumido(GameObject itemDaLista)
    {
        ItemConsumo itemExcluir = itemDaLista.GetComponent<ItemConsumido>().GetItem();
        _consumoTotal.ListaConsumo.Remove(itemExcluir);

        Destroy(itemDaLista);
        AtualizarConsumoTotal();
    }
}
