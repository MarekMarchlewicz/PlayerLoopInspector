#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.LowLevel;
using UnityEngine.UIElements;

namespace Utilities
{
    internal class PlayerLoopInspector : EditorWindow
    {
        const string k_Name = "Player Loop Inspector";

        [MenuItem("Window/Player Loop Inspector")]
        static void ShowWindow()
        {
            var instance = GetWindow<PlayerLoopInspector>(false, k_Name);
            instance.Show();
        }

        static bool showCurrent
        {
            get => EditorPrefs.GetBool(k_Name + nameof(showCurrent), true);
            set => EditorPrefs.SetBool(k_Name + nameof(showCurrent), value);
        }

        TreeView m_TreeView;

        void OnEnable()
        {
            Refresh();
        }

        void Refresh()
        {
            rootVisualElement.Clear();

            var buttonsContainer = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    justifyContent = Justify.SpaceBetween,
                    marginLeft = 5, marginRight = 5, marginTop = 5, marginBottom = 5
                }
            };
            var loopSelectionGroup = new VisualElement { style = { flexDirection = FlexDirection.Row } };
            loopSelectionGroup.Add(new Button(ShowCurrent) { text = "Current" });
            loopSelectionGroup.Add(new Button(ShowDefault) { text = "Default" });
            buttonsContainer.Add(loopSelectionGroup);

            var refreshButton = new Button(Refresh) { text = "Reload" };
            buttonsContainer.Add(refreshButton);

            rootVisualElement.Add(buttonsContainer);

            var loop = showCurrent ? PlayerLoop.GetCurrentPlayerLoop() : PlayerLoop.GetDefaultPlayerLoop();
            var id = 0;
            var rootItems = GetRootItems(ref id, loop.subSystemList);
            m_TreeView = new TreeView(MakeItem, BindItem) { fixedItemHeight = 17 };
            m_TreeView.SetRootItems(rootItems);
            rootVisualElement.Add(m_TreeView);
        }

        void ShowCurrent()
        {
            showCurrent = true;
            Refresh();
        }

        void ShowDefault()
        {
            showCurrent = false;
            Refresh();
        }

        static List<TreeViewItemData<PlayerLoopSystem>> GetRootItems(ref int id, IList<PlayerLoopSystem> systems)
        {
            if (systems == null)
                return new List<TreeViewItemData<PlayerLoopSystem>>();

            var items = new List<TreeViewItemData<PlayerLoopSystem>>(systems.Count);
            foreach (var playerLoopSystem in systems)
                items.Add(new TreeViewItemData<PlayerLoopSystem>(id++, playerLoopSystem, GetRootItems(ref id, playerLoopSystem.subSystemList)));
            return items;
        }

        void BindItem(VisualElement visualElement, int index)
        {
            var playerLoopSystem = m_TreeView.GetItemDataForIndex<PlayerLoopSystem>(index);
            visualElement.Q<Label>().text = playerLoopSystem.type.Name;
        }

        static VisualElement MakeItem() => new Label { style = { paddingTop = 1, paddingBottom = 1 } };
    }
}
#endif
