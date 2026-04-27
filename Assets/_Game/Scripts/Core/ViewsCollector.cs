using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sling.Core
{
    public class ViewsCollector
    {
        private readonly Dictionary<Type, List<BaseView>> _viewsByType = new();

        public void CollectViews(Scene scene)
        {
            _viewsByType.Clear();

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                foreach (BaseView view in root.GetComponentsInChildren<BaseView>())
                {
                    foreach (Type type in view.GetTypesToRegister())
                    {
                        if (!_viewsByType.TryGetValue(type, out List<BaseView> views))
                        {
                            views = new List<BaseView>();
                            _viewsByType.Add(type, views);
                        }

                        views.Add(view);
                    }
                }
            }
        }

        public T GetOne<T>() where T : BaseView => 
            GetAll<T>().FirstOrDefault();

        public List<T> GetAll<T>() where T : BaseView
        {
            if(!_viewsByType.TryGetValue(typeof(T), out List<BaseView> list))
                return new List<T>();
        
            return list.ConvertAll(v => (T) v);
        }
    }
}
