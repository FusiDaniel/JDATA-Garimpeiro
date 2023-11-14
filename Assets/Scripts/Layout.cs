using System.Collections; 
using System.Collections.Generic; 
using UnityEngine;
using System.Xml.Serialization;

// Esta classe SlotDef não é uma subclasse (não herda) de MonoBehaviour, é um Objeto Scriptável
// um arquivo separado. Não precisa ficar com os .cs comuns do projeto.
[System.Serializable] 
// Torna SlotDefs visíveis no Inspector
public class SlotDef { 
    public float x; 
    public float y;
    public bool faceUp = false; 

    public string layerName = "Default"; 
    public int layerID = 0; 
    public int id;
    public List<int> hiddenBy = new List<int>(); 
    public string type = "slot"; 
    public Vector2 espaco;
}
// Esta classe Layout já faz parte "normal" da Herança dos scripts do projeto
public class Layout : MonoBehaviour { 
    public PT_XMLReader xmlr; // Guarda o PT_XMLReader 
    public PT_XMLHashtable xml;  // acelera o acesso xml
    public Vector2 multiplicador; // Offset do centro do tabuleiro
    // Refrências de SlotDef
    public List<SlotDef> slotDefs; // Todas as SlotDefs para Row0-Row3
    public SlotDef monte; 
    public SlotDef descarte;
    // Armazena todas os nomes possíveis das Layers dadas por layerID 
    public string[] sortingLayerNames = new string[] { "Row0", "Row1", "Row2", "Row3", "Monte", "Descarte" };
    // Este método é chamado para ler o arquivo LayoutXML.xml
    public void ReadLayout(string xmlText) { 
        xmlr = new PT_XMLReader(); 
        xmlr.Parse(xmlText);  // O XML é "parsed" 
        xml = xmlr.xml["xml"][0];  // xml é atribuído como shortcut para o XML
        // Lê o percentual multiplicador, transformando x e y em decimal        
        multiplicador.x = (float) int.Parse(xml["multiplicador"][0].att("x"))/100.0f; 
        multiplicador.y = (float) int.Parse(xml["multiplicador"][0].att("y"))/100.0f; 
        SlotDef tSD; // slotsX é usado para todos os <slot>s
        PT_XMLHashList slotsX = xml["slot"]; // Leitura nos slots
        for (int i=0; i<slotsX.Count; i++) { 
            tSD = new SlotDef(); // Cria uma nova instância SlotDef            
            if (slotsX[i].HasAtt("type")) {
                tSD.type = slotsX[i].att("type"); // Se o <slot> tem um type "parse"-o             
            }                
            else {
                tSD.type = "slot"; // Se não o tem, atribui type = "slot"; É uma carta das linhas               
            }
            tSD.x = float.Parse( slotsX[i].att("x") ); // Posições das cartas 
            tSD.y = float.Parse( slotsX[i].att("y") ); 
            tSD.layerID = int.Parse( slotsX[i].att("layer") );             
            tSD.layerName = sortingLayerNames[ tSD.layerID ];  // Converte o nº layerID num layerName string
            
            switch (tSD.type) { // seleção de atributos de acordo com o tipo de <slot>                 
                case "slot": 
                    tSD.faceUp = (slotsX[i].att("faceup") == "1"); 
                    tSD.id = int.Parse( slotsX[i].att("id") ); 
                    if (slotsX[i].HasAtt("hiddenby")) { 
                        string[] hiding = slotsX[i].att("hiddenby").Split(','); 
                        foreach( string s in hiding ) { tSD.hiddenBy.Add ( int.Parse(s) ); }
                    } 
                    slotDefs.Add(tSD); 
                    break;
                case "monte": 
                    tSD.espaco.x = (float) int.Parse( slotsX[i].att("espaco_x") )/100.0f;                     
                    monte = tSD; 
                    break;
                case "descarte": 
                    descarte = tSD; 
                    break;
            }
        }
    }
}
