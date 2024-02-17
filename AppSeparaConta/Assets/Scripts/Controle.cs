using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

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
    [Header("Edicao itens")]
    public TMP_Dropdown dropEditDivididoEm;
    public TMP_Dropdown dropEditPagarPartes;
    public TMP_Text txtEditSubValorItem;
    [Space]
    public GameObject painelCadCardapio;
    public GameObject painelEditItem;
    [Space]
    public Transform painelContentLista;
    public GameObject prefabItemLista;

    [SerializeField]
    private ConsumoTotal _consumoTotal = new ConsumoTotal();
    private List<ItemCardapio> cardapio = new List<ItemCardapio>();
    private ItemCardapio itemPreselecionado = new ItemCardapio();
    private GameObject itemParaEditar = null;

    public static Controle SINGLETON = null;

    private string nomeBackup = "/saveData.txt";

    void Start()
    {
        if(SINGLETON == null)
        {
            SINGLETON = this;
        }

        PopulaDrops();
        AtualizarItensDropCardapio();
        onclick_fecharPainelCadCardapio();
        onclick_fecharPainelEditItem();
    }
    
    void Update()
    {
        
    }

    private void PopulaDrops()
    {
        dropDivididoEm.ClearOptions();
        dropPagarPartes.ClearOptions();

        dropEditDivididoEm.ClearOptions();
        dropEditPagarPartes.ClearOptions();

        List<string> lista = new List<string>();
        for (int i = 1; i < 21; i++)
        {
            lista.Add(i.ToString());
        }
        dropDivididoEm.AddOptions(lista);
        dropEditDivididoEm.AddOptions(lista);

        onchange_mudouDivididoEm(0);
        onchange_mudouEditDivididoEm(0);
    }

    public void onclick_recuperarBackup()
    {
        string relPath = Application.persistentDataPath + nomeBackup;

        if (File.Exists(relPath))
        {
            string retorno = File.ReadAllText(relPath);
            ConsumoTotal backup = JsonConvert.DeserializeObject<ConsumoTotal>(retorno);

            cardapio = backup.Cardapio;

            AtualizarItensDropCardapio();

            foreach (var item in backup.ListaConsumo)
            {
                AdicionarListaConsumo(item);
            }
        }
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

            _consumoTotal.Cardapio = cardapio;
        }


    }

    public void SetItemParaeditar(GameObject gm)
    {
        itemParaEditar = gm;

        ItemConsumido paraEditar = itemParaEditar.GetComponent<ItemConsumido>();
        
        AtualizaDadosItemTelaEdit();
        
        dropEditDivididoEm.value = paraEditar.GetItem().DivididoEm - 1;
        dropEditPagarPartes.value = paraEditar.GetItem().PartesPagas - 1;

        onclick_abrirPainelEditItem();
    }

    /// <summary>
    /// Atualiza o dropdown de item do cardapio baseado na lista List<ItemCardapio>
    /// </summary>
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

    public void onchange_mudouEditDivididoEm(int index)
    {
        dropEditPagarPartes.options.Clear();
        List<string> lista = new List<string>();
        for (int i = 0; i < dropEditDivididoEm.value + 1; i++)
        {
            lista.Add($"{i + 1}/{dropEditDivididoEm.value + 1}");
        }
        dropEditPagarPartes.AddOptions(lista);
        AtualizaDadosItemTelaEdit();
    }

    public void onchange_mudouEditPartesPagas(int index)
    {
        AtualizaDadosItemTelaEdit();
    }


    private ItemConsumo AtualizaDadosItemTela()
    {
        txtValorTotalItem.text = string.Format("{0:C2}", itemPreselecionado.PrecoTotal);
        ItemConsumo itemConsumo = new ItemConsumo() { DivididoEm = dropDivididoEm.value + 1, PartesPagas = dropPagarPartes.value + 1, ItemDoCardapio = itemPreselecionado };
        txtSubValorItem.text = string.Format("{0:C2}", itemConsumo.GetSubTotal());

        return itemConsumo;
    }

    private ItemConsumo AtualizaDadosItemTelaEdit()
    {
        if(itemParaEditar == null)
        {
            return null;
        }
        ItemConsumo itemConsumo = new ItemConsumo() { DivididoEm = dropEditDivididoEm.value + 1, PartesPagas = dropEditPagarPartes.value + 1, ItemDoCardapio = itemParaEditar.GetComponent<ItemConsumido>().GetItem().ItemDoCardapio };
        itemConsumo.Id = itemParaEditar.GetComponent<ItemConsumido>().GetItem().Id;

        txtEditSubValorItem.text = string.Format("{0:C2}", itemConsumo.GetSubTotal());

        return itemConsumo;
    }

    public void onclick_adicionarConsumo()
    {
        ItemConsumo item = AtualizaDadosItemTela();
        _consumoTotal.ListaConsumo.Add(item);
        print("Adicionado " + item.ToString());

        AdicionarListaConsumo(item);
    }

    private void AdicionarListaConsumo(ItemConsumo item)
    {
        GameObject copia = Instantiate(prefabItemLista, painelContentLista.position, Quaternion.identity, painelContentLista);
        copia.GetComponent<ItemConsumido>().SetValores(item);

        AtualizarConsumoTotal();
    }

    private void AtualizarConsumoTotal()
    {
        txtConsumoTotal.text = string.Format("Total: {0:C2}", _consumoTotal.GetConsumoTotal());

        SalvarJson();
    }

    public void onclick_abrirPainelCadCardapio()
    {
        painelCadCardapio.SetActive(true);
    }

    public void onclick_fecharPainelCadCardapio()
    {
        painelCadCardapio.SetActive(false);
    }

    public void onclick_abrirPainelEditItem()
    {
        dropDivididoEm.value = itemParaEditar.GetComponent<ItemConsumido>().GetItem().DivididoEm - 1;

        painelEditItem.SetActive(true);
    }

    public void onclick_updateItemConsumido()
    {
        ItemConsumo item = AtualizaDadosItemTelaEdit();

        ItemConsumo itemEditado = _consumoTotal.ListaConsumo.Where(x => x.Id == item.Id).FirstOrDefault();

        if(itemEditado != null)
        {
            itemEditado.PartesPagas = item.PartesPagas;
            itemEditado.DivididoEm = item.DivididoEm;

            print("Editado " + itemEditado.ToString());

            itemParaEditar.GetComponent<ItemConsumido>().SetValores(item);

            AtualizarConsumoTotal();
        }

        onclick_fecharPainelEditItem();
    }

    public void onclick_fecharPainelEditItem()
    {
        painelEditItem.SetActive(false);
    }

    public void deletarItemConsumido(GameObject itemDaLista)
    {
        ItemConsumo itemExcluir = itemDaLista.GetComponent<ItemConsumido>().GetItem();
        _consumoTotal.ListaConsumo.Remove(itemExcluir);

        Destroy(itemDaLista);
        AtualizarConsumoTotal();
    }

    private void SalvarJson()
    {
        string result = JsonConvert.SerializeObject(_consumoTotal);

        string relPath = Application.persistentDataPath + nomeBackup;

        if (!File.Exists(relPath))
        {
            File.Create(relPath);
        }

        File.WriteAllText(relPath, result);

        print("Salvo em: " + relPath);

        //ConsumoTotal backup = JsonConvert.DeserializeObject<ConsumoTotal>(result);

    }
}
