using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Utils 
{
    /// <summary>
    /// Função que serve para testar se um objeto "vê" o outro, ou seja se não há obstáculos entre dois objetos
    /// </summary>
    /// <param name="positionFrom">Transform do objeto inicial</param>
    /// <param name="positionTo">Transform do objeto final</param>
    /// <param name="tag">Opcional: tag do objeto a ver</param>
    /// <param name="fov">Opcional: campo de visão</param>
    /// <param name="MaxDistance">Opcional: distância máxima</param>
    /// <param name="IgnoreMask">Opcional: Layer a ignorar</param>
    /// <returns>true se não existem obstáculos ou false se existem</returns>
    public static bool CanYouSeeThis(Transform positionFrom, Transform positionTo, string tag = null, float fov = 0, float MaxDistance = 0,string IgnoreLayer="")
    {
        RaycastHit hit;
        LayerMask mask=LayerMask.GetMask(IgnoreLayer);
        if(IgnoreLayer=="")
            mask=~0;
        else
            mask = ~mask;
       // Debug.Log(mask.value);
        if (fov == 360) fov = 0;
        //testa distancia
        var distance = Vector3.Distance(positionFrom.position, positionTo.position);
        if (MaxDistance > 0 && distance > MaxDistance)
        {
           // Debug.Log("Muito longe");
            return false;  //muito longe
        }
        //direção
        var rayDirection = positionTo.position - positionFrom.position;
        Debug.DrawRay(positionFrom.position, rayDirection, Color.blue);
        if (Physics.Raycast(positionFrom.position, rayDirection, out hit,Mathf.Infinity,mask))
        {
           // Debug.Log("hit " + hit.transform.name);
            //não testa tag nem fov
            if (string.IsNullOrEmpty(tag) && fov == 0) return (hit.transform.root.gameObject==positionTo.root.gameObject);

           // Debug.Log("hit tag " + hit.transform.tag);
            //testa tag não testa fov
            if (tag!=null && fov == 0) return hit.transform.tag.Equals(tag);
            
            //cálcula angulo
            float angle = Vector3.Angle(positionTo.position - positionFrom.position, positionFrom.forward);
           // Debug.Log("Angulo " + angle);

            //testa fov não testa tag
            if (tag == null && fov != 0)
            {
                if (angle <= fov && (hit.transform.gameObject == positionTo.gameObject))
                    return true;
                else
                    return false;
            }
           // Debug.Log(hit.transform.gameObject);
            //testa tag e fov
         //   Debug.Log("Testa tag e fov");
            if (hit.transform.tag.Equals(tag) && angle <= fov && (hit.transform.root.gameObject == positionTo.root.gameObject))
                return true;
            return false;// (hit.transform.position == positionTo);
        }
      //  Debug.Log("Não vejo nada");
        return false;
    }
    /// <summary>
    /// Função que devolve um vetor com os componentes dos filhos sem o pai
    /// A função que existe do Unity devolve sempre o pai
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="obj">Parent game object</param>
    /// <returns>Devolve um vetor com os componentes dos filhos sem incluir o pai</returns>
    public static T[] GetComponentsInChildWithoutRoot<T>(GameObject obj) where T : Component
    {
        List<T> tList = new List<T>();
        if (obj == null) return null;
        foreach (Transform child in obj.transform)
        {
            T[] scripts = child.GetComponentsInChildren<T>();
            if (scripts != null)
            {
                foreach (T sc in scripts)
                    tList.Add(sc);
            }
        }
        return tList.ToArray();
    }
	/// <summary>
    /// Função que devolve um vetor com os componentes dos filhos sem o pai
    /// A função que existe do Unity devolve sempre o pai
    /// </summary>
    /// <typeparam name="T">Object type</typeparam>
    /// <param name="obj">Parent Game Object</param>
    /// <param name="tag">Only returns objects with this tag</param>
    /// <returns>Devolve um vetor com os componentes dos filhos sem incluir o pai</returns>
    public static T[] GetComponentsInChildWithoutRoot<T>(GameObject obj, string tag) where T : Component {
        List<T> tList = new List<T>();
        foreach (Transform child in obj.transform.root) {
            T[] scripts = child.GetComponentsInChildren<T>();
            if (scripts != null) {
                foreach (T sc in scripts)
                    if (sc.tag == tag) tList.Add(sc);
            }
        }
        return tList.ToArray();
    }
    /// <summary>
    /// Devolve o componente se existir na hierarquia do gameobject
    /// seja no mesmo, nos filhos ou no parent
    /// </summary>
    /// <typeparam name="T">Tipo de componente</typeparam>
    /// <param name="gameObject">GameObject a pesquisar</param>
    /// <returns>null se não encontra</returns>
    public static T GetComponentAtAnyCost<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();
        if (component != null) return component;
        component = gameObject.GetComponentInChildren<T>();
        if (component != null ) return component;
        component = gameObject.GetComponentInParent<T>();
        if (component != null) return component;
        return null;
    }
	
	/// <summary>
    /// Função para indicar se um objeto está à direita >0 ou à esquerda <0 de outro
    /// </summary>
    /// <param name="fwd"></param>
    /// <param name="targetDir"></param>
    /// <param name="up"></param>
    /// <returns><0 para esquerda; >0 para direita; 0 para em frente</returns>
    public static float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up) {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
        return dir;
    }


    /// <summary>
    /// Converte um valor numa cor 
    /// 8 bits para R mais 8 bits para G mais 8 bits para B
    /// </summary>
    /// <param name="value">Valor da cor</param>
    /// <returns>Cor correspondente ao valor</returns>
    public static Color ToColor(int value) {
        return new Color(((value & 0xFF0000) >> 16) / 255f,
                        ((value & 0xFF00) >> 8) / 255f,
                        (value & 0xFF) / 255f);
    }
    /// <summary>
    /// Faz uma cópia de um componente e adiciona-o a outro gameobject
    /// se não existir o destino (null) é criado um
    /// </summary>
    /// <typeparam name="T">Tipo de componente a copiar</typeparam>
    /// <param name="original">Componente a copiar</param>
    /// <param name="destination">GameObject que recebe o componente copiado</param>
    /// <returns>Cópia do componente</returns>
    public static T CopyComponent<T>(T original, GameObject destination) where T : Component {
        System.Type type = original.GetType();
        Component copy;
        if (destination == null)
            destination= new GameObject();
        copy = destination.AddComponent(type);

        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields) {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }
    /// <summary>
    /// Para um NavMeshAgent 
    /// </summary>
    /// <param name="navMeshAgent"></param>
	public static void StopNavMeshAgent(NavMeshAgent navMeshAgent)
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
    }
    /// <summary>
    /// Devolve um ou menos 1
    /// </summary>
    /// <returns>1 ou -1</returns>
    public static int RandomOneMinusOne()
    {
        int n = Random.Range(0, 100);
        if (n >= 50)
            return 1;
        return -1;
    }
    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }
    public static string ToHex(Color c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }
    public static string AddColorTag(string text,Color color)
    {
        string output;
        output = string.Format("<color={0}>{1}</color>", ToHex(color), text);
        return output;
    }
    public static void LogWithColor(string text, Color? color=null)
    {
        Color textColor;
        if (color == null) { textColor = Color.black; }
        else textColor = (Color)color;
        Debug.Log(AddColorTag(text, textColor));
    }
}
