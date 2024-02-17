using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;

public class Controle : MonoBehaviour
{
    public TMP_InputField inputNomeRole;
    [Space]
    [Header("Cardapio")]
    public TMP_InputField inputNomeItemCardapio;
    public TMP_InputField inputPrecoItemCardapio;
    [Space]
    [Header("Lista consumo")]
    public Button btnAdicionar;
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
    public TMP_Text txtEditNomeItem;
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
        btnAdicionar.interactable = false;
        dropDivididoEm.interactable = false;
        dropPagarPartes.interactable = false;
    }
    
    void Update()
    {
        
    }

    /// <summary>
    /// Preenche os dropdown com os valores iniciais
    /// </summary>
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

    /// <summary>
    /// Recuepra o json salvo no arquivo texto e preenche a tela com os dados salvos
    /// </summary>
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

    /// <summary>
    /// Adiciona um novo item do painel de adicionar itens na lista e dropdown para ser consumido na tela principal
    /// </summary>
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

    /// <summary>
    /// Seta os valores no painel de edição de consumo
    /// </summary>
    /// <param name="gm"></param>
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

        dropItensCardapio.value = 0;
        onchange_mudouItemDropCardapio(0);


    }

    /// <summary>
    /// Executado quando usuario troca opção no dropdown de item a ser consumido
    /// </summary>
    /// <param name="index"></param>
    public void onchange_mudouItemDropCardapio(int index)
    {

        if(index > 0)
        {
            ItemCardapio itemSelecionado = cardapio[index - 1];
            if (itemSelecionado != null)
            {
                itemPreselecionado = itemSelecionado;
                AtualizaDadosItemTela();
            }

            dropDivididoEm.interactable = true;
            dropPagarPartes.interactable = true;
            btnAdicionar.interactable = true;
        }
        else
        {
            txtValorTotalItem.text = string.Format("{0:C2}", 0);
            txtSubValorItem.text = string.Format("{0:C2}", 0);
            dropDivididoEm.interactable = false;
            dropPagarPartes.interactable = false;
            btnAdicionar.interactable = false;
            dropPagarPartes.SetValueWithoutNotify(0);
            dropDivididoEm.SetValueWithoutNotify(0);
        }
    }

    /// <summary>
    /// Executa quando o usuario troca o valor do dropdown que define em quantas partes sera dividido o valor do item
    /// </summary>
    /// <param name="index"></param>
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

    /// <summary>
    /// Executado quando o valor do dropdown que define quantas partes serão pagas é alterado
    /// </summary>
    /// <param name="index"></param>
    public void onchange_mudouPartesPagas(int index)
    {
        AtualizaDadosItemTela();
    }

    /// <summary>
    /// Acontece quando o usuario troca de opção no dropdown de seleção de divisao de valor na tela de edição do item
    /// </summary>
    /// <param name="index"></param>
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

    /// <summary>
    /// Acontece quando o usuario troca de opção no dropdown de seleção de quatidade a pagar na tela de edição do item
    /// </summary>
    /// <param name="index"></param>
    public void onchange_mudouEditPartesPagas(int index)
    {
        AtualizaDadosItemTelaEdit();
    }

    /// <summary>
    /// Atualiza os textos na tela de acordo com o valor do item selecionado para ser adicionado na lista de consumo
    /// </summary>
    /// <returns></returns>
    private ItemConsumo AtualizaDadosItemTela()
    {
        txtValorTotalItem.text = string.Format("{0:C2}", itemPreselecionado.PrecoTotal);
        ItemConsumo itemConsumo = new ItemConsumo() { DivididoEm = dropDivididoEm.value + 1, PartesPagas = dropPagarPartes.value + 1, ItemDoCardapio = itemPreselecionado };
        txtSubValorItem.text = string.Format("{0:C2}", itemConsumo.GetSubTotal());

        return itemConsumo;
    }

    /// <summary>
    /// Atualiza os dados na tela de edição e retorna o dado atualizado
    /// </summary>
    /// <returns></returns>
    private ItemConsumo AtualizaDadosItemTelaEdit()
    {
        if(itemParaEditar == null)
        {
            return null;
        }
        ItemConsumo itemConsumo = new ItemConsumo() { DivididoEm = dropEditDivididoEm.value + 1, PartesPagas = dropEditPagarPartes.value + 1, ItemDoCardapio = itemParaEditar.GetComponent<ItemConsumido>().GetItem().ItemDoCardapio };
        itemConsumo.Id = itemParaEditar.GetComponent<ItemConsumido>().GetItem().Id;

        txtEditSubValorItem.text = string.Format("{0:C2}", itemConsumo.GetSubTotal());
        txtEditNomeItem.text = string.Format("{0} - {1:C2}", itemConsumo.ItemDoCardapio.Nome, itemConsumo.ItemDoCardapio.PrecoTotal) ;
        
        return itemConsumo;
    }

    /// <summary>
    /// Executando quando o usuario clica no botao para adicionar consumo
    /// </summary>
    public void onclick_adicionarConsumo()
    {
        ItemConsumo item = AtualizaDadosItemTela();
        _consumoTotal.ListaConsumo.Add(item);
        print("Adicionado " + item.ToString());

        AdicionarListaConsumo(item);
    }

    /// <summary>
    /// Adiciona o prefab na lista de consumo com o item adicionado
    /// </summary>
    /// <param name="item"></param>
    private void AdicionarListaConsumo(ItemConsumo item)
    {
        GameObject copia = Instantiate(prefabItemLista, painelContentLista.position, Quaternion.identity, painelContentLista);
        copia.GetComponent<ItemConsumido>().SetValores(item);

        AtualizarConsumoTotal();
    }

    /// <summary>
    /// Atualiza o texto com valor total do consumo
    /// </summary>
    private void AtualizarConsumoTotal()
    {
        txtConsumoTotal.text = string.Format("Total: {0:C2}", _consumoTotal.GetConsumoTotal());

        SalvarJson();
    }

    /// <summary>
    /// mostra o painel de adicionar item no cardapio
    /// </summary>
    public void onclick_abrirPainelCadCardapio()
    {
        painelCadCardapio.SetActive(true);
    }

    /// <summary>
    /// Esconde o painel de adicionar item no cardapio
    /// </summary>
    public void onclick_fecharPainelCadCardapio()
    {
        painelCadCardapio.SetActive(false);
    }

    /// <summary>
    /// Abe o painel de edição de item
    /// </summary>
    public void onclick_abrirPainelEditItem()
    {
        dropDivididoEm.value = itemParaEditar.GetComponent<ItemConsumido>().GetItem().DivididoEm - 1;

        painelEditItem.SetActive(true);
    }

    /// <summary>
    /// Atualiza os dados do item selecionado para ser editado
    /// </summary>
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

    /// <summary>
    /// Esconde o painel de edição de item
    /// </summary>
    public void onclick_fecharPainelEditItem()
    {
        painelEditItem.SetActive(false);
    }

    /// <summary>
    /// Deleta o item da lista de consumo e da tela
    /// </summary>
    /// <param name="itemDaLista"></param>
    public void deletarItemConsumido(GameObject itemDaLista)
    {
        ItemConsumo itemExcluir = itemDaLista.GetComponent<ItemConsumido>().GetItem();
        if(itemExcluir != null)
        {
            ItemConsumo buscaParaExcluir = _consumoTotal.ListaConsumo.Where(x => x.Id == itemExcluir.Id).FirstOrDefault();
            if(buscaParaExcluir != null)
            {
                bool result = _consumoTotal.ListaConsumo.Remove(buscaParaExcluir);
                if (result)
                {
                    print("Excluir item com id " + itemExcluir.Id);
                    AtualizarConsumoTotal();
                    Destroy(itemDaLista);
                }
                else
                {
                    print("Falha ao excluir");
                }
                
            }
        }
        else
        {
            print("Nao acho item para excluir");
        }
    }

    /// <summary>
    /// Salva o json do objeto principal em formato txt
    /// </summary>
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
