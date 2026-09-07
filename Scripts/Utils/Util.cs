using System.IO;
using System.Text;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// (주의 무거움) = Update에서 쓰기에는 무리 정도로 이해하면 Good
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Game Object에서 해당 Component 얻거나 없으면 추가 (주의 무거움)
        /// </summary>
        public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
        {
            return go.GetComponent<T>() ?? go.AddComponent<T>();
        }

        /// <summary>
        /// Game Object의 자식 중 T 컴포넌트를 가진 자식 얻기 (주의 무거움)
        /// </summary>
        /// <param name="go"> 부모 객체 </param>
        /// <param name="name">자식의 이름</param>
        /// <param name="recursive">재귀적 탐색 여부</param>
        public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
        {
            if (go == null) return null;

            if (recursive == false)
            {
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    Transform child = go.transform.GetChild(i);
                    if (string.IsNullOrEmpty(name) || child.name == name)
                    {
                        T component = child.GetComponent<T>();
                        if (component != null)
                            return component;
                    }
                }
            }
            else
            {
                foreach (T child in go.GetComponentsInChildren<T>())
                    if (string.IsNullOrEmpty(name) || child.name == name)
                        return child;
            }

            return null;
        }
        /// <summary>
        /// Game Object의 부모 중 T 컴포넌트를 가진 부모 얻기 (주의 무거움)
        /// </summary>
        /// <param name="go"> 부모 객체 </param>
        /// <param name="name">자식의 이름</param>
        /// <param name="recursive"> 재귀적 탐색 여부</param>
        public static T FindParent<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Component
        {
            if (go == null) return null;

            Transform parentTransform = go.transform.parent;

            if (parentTransform == null)
                return null;

            if (recursive == false)
            {
                T traget = parentTransform.GetComponent<T>();
                return traget;
            }
            else
            {
                T traget = parentTransform.GetComponent<T>();
                if (traget == null)
                {
                    return FindParent<T>(parentTransform.gameObject, name, recursive);
                }
                return traget;
            }

            return null;
        }

        /// <summary>
        /// Game Object 전용 FindChild (주의 무거움)
        /// </summary>
        public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
        {
            Transform transform = FindChild<Transform>(go, name, recursive);
            if (transform == null) return null;
            return transform.gameObject;
        }




        ///////////////////////////////////////////
        /// <summary>
        /// Json 정보를 Write하는 방법 (주의 무거움)
        /// UNITY_EDITOR 와 UNITY_ANDROID를 기준으로 작성
        /// </summary>
        /// <typeparam name="Handler"> Data Type </typeparam>
        /// <param name="_data"> 저장할 데이터 </param>
        /// <param name="_name"> 파일명, default : {0}Handler 일때 파일 명을 {0}s.json / 해당 형식이 아닌 경우 그냥 Class 명 </param>
        public static void SaveJson<Handler>(Handler _data, string _name = null)
        {
            string _jsonText;

            if (string.IsNullOrEmpty(_name))
            {
                _name = typeof(Handler).Name;

                int Index = _name.IndexOf("Handler");
                if (Index != -1)
                {
                    _name = string.Concat(_name.Substring(0, Index), 's');
                }
            }

            //안드로이드에서의 저장 위치를 다르게 해주어야 한다
            //안드로이드의 경우에는 데이터조작을 막기위해 2진데이터로 변환을 해야한다

            string _savePath;
            string _appender = "/userData/";
            string _nameString = _name + ".json";

#if UNITY_EDITOR
            _savePath = Application.dataPath;
#elif UNITY_ANDROID
        _savePath = Application.persistentDataPath;
#endif


            StringBuilder _builder = new StringBuilder(_savePath);
            _builder.Append(_appender);
            if (!Directory.Exists(_builder.ToString()))
            {
                //디렉토리가 없는경우 만들어준다
                Debug.Log("No Directory");
                Directory.CreateDirectory(_builder.ToString());

            }
            _builder.Append(_nameString);

            _jsonText = JsonUtility.ToJson(_data, true);
            Debug.Log(_jsonText);

            using (FileStream _fileStream = new FileStream(_builder.ToString(), FileMode.Create))
            {
                byte[] _bytes = Encoding.UTF8.GetBytes(_jsonText);
                _fileStream.Write(_bytes, 0, _bytes.Length);
                _fileStream.Close();
            }

        }

        /// <summary>
        /// Json 정보를 Read하는 방법 (주의 무거움)
        /// UNITY_EDITOR 와 UNITY_ANDROID를 기준으로 작성
        /// </summary>
        /// <typeparam name="Handler"> Data Type </typeparam>
        /// <param name="_name"> 파일명, default : {0}Handler 일때 파일 명을 {0}s.json / 해당 형식이 아닌 경우 그냥 Class 명 </param>
        /// <returns></returns>
        public static Handler LoadSaveData<Handler>(string _name = null)
        {
            if (string.IsNullOrEmpty(_name))
            {
                _name = typeof(Handler).Name;

                int Index = _name.IndexOf("Handler");
                if (Index != -1)
                {
                    _name = string.Concat(_name.Substring(0, Index), 's');
                }
            }
            Handler _gameData;
            string _loadPath;
            string _directory = "/userData";
            string _appender = "/";
            string _dotJson = ".json";

#if UNITY_EDITOR
            _loadPath = Application.dataPath;
#elif UNITY_ANDROID
        _loadPath = Application.persistentDataPath;
#endif

            StringBuilder _builder = new StringBuilder(_loadPath);
            _builder.Append(_directory);

            string _builderToString = _builder.ToString();
            if (!Directory.Exists(_builderToString))
            {
                Directory.CreateDirectory(_builderToString);

            }
            _builder.Append(_appender);
            _builder.Append(_name);
            _builder.Append(_dotJson);

            if (File.Exists(_builder.ToString()))
            {
                //세이브 파일이 있는경우

                using (FileStream _stream = new FileStream(_builder.ToString(), FileMode.Open))
                {
                    byte[] _bytes = new byte[_stream.Length];
                    _stream.Read(_bytes, 0, _bytes.Length);
                    _stream.Close();
                    string _jsonData = Encoding.UTF8.GetString(_bytes);
                    //텍스트를 string으로 바꾼다음에 FromJson에 넣어주면은 우리가 쓸 수 있는 객체로 바꿀 수 있다
                    _gameData = JsonUtility.FromJson<Handler>(_jsonData);
                }

            }
            else
            {
                //세이브파일이 없는경우
                _gameData = default(Handler);
            }
            return _gameData;
        }

        /// <summary>
        /// Raycast 마우스 포인터 기반 2D 오브젝트 획득
        /// </summary>
        /// <param name="LayerMask"> LayerMask </param>
        /// <returns></returns>
        public static GameObject GetObjRaycast2D(int LayerMask = (1 << 0))
        {
            GameObject target = null;

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Ray2D ray = new Ray2D(pos, Vector2.zero);
            RaycastHit2D hit;
            hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask);

            if (hit) //마우스 근처에 오브젝트가 있는지 확인
            {
                //있으면 오브젝트를 저장한다.
                target = hit.collider.gameObject;
            }
            return target;
        }
        /// <summary>
        /// Raycast 마우스 포인터 기반 3D 오브젝트 획득
        /// </summary>
        /// <param name="LayerMask"> LayerMask </param>
        /// <returns></returns>
        public static GameObject GetObjRaycast3D(int LayerMask = (1 << 0))
        {
            GameObject target = null;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask))
            {
                target = hit.transform.gameObject;
                // Do something with the object that was hit by the raycast.
            }

            return target;
        }

        public static Color GetHexColor(string hexCode)
        {
            if (ColorUtility.TryParseHtmlString(hexCode, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogWarning($"Invalid Hex Code: {hexCode}");
                return Color.white; // 기본값 (잘못된 Hex 코드가 들어오면 흰색 반환)
            }
        }

    }
}