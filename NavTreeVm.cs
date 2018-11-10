using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AppBase;
using Modelss;
using APPBASE;
using SUVI_APP.ViewModels.TreeviewViewModel.TreeviewClass;


namespace SUVI_APP.ViewModels.TreeviewViewModel 
{
    // qq properties for demo one tree

    public class NavTreeVm : BaseViewModel
    {
        public NavTreeItem treeRootItem;
        // public ICommand SelectedPathFromTreeCommand moved to ViewModel

        public ObservableCollection<ListBoxImageModel> BingingImage { get; set; }

        public DelegateCommand selectedPathFromTreeCommand;
        // a Name to bind to the NavTreeTabs
        private string treeName = "";
        public string TreeName
        {
            get { return treeName; }
            set { SetProperty(ref treeName, value, "TreeName"); }
        }

        // RootNr determines nr of RootItem that is used as RootNode 
        private int rootNr;
        public int RootNr
        {
            get { return rootNr; }
            set { SetProperty(ref rootNr, value, "RootNr"); }
        }

        // RootChildren are used to bind to TreeView
        private ObservableCollection<INavTreeItem> rootChildren = new ObservableCollection<INavTreeItem> { };
        public ObservableCollection<INavTreeItem> RootChildren
        {
            get { return rootChildren; }
            set { SetProperty(ref rootChildren, value, "RootChildren"); }
        }

        public void RebuildTree(int pRootNr = -1, bool pIncludeFileChildren = false)
        {
            // First take snapshot of current expanded items
            List<String> SnapShot = NavTreeUtils.TakeSnapshot(rootChildren);

            // As a matter of fact we delete and construct the tree//RoorChildren again.....
            // Delete all rootChildren
            foreach (INavTreeItem item in rootChildren) item.DeleteChildren();
            rootChildren.Clear();

            // Create treeRootItem 
            if (pRootNr != -1) RootNr = pRootNr;
             treeRootItem = NavTreeRootItemUtils.ReturnRootItem(RootNr, pIncludeFileChildren);
            if (pRootNr != -1) TreeName = treeRootItem.FriendlyName;

            // Copy children treeRootItem to RootChildren, set up new tree 
            foreach (INavTreeItem item in treeRootItem.Children) { RootChildren.Add(item); }

            //Expand previous snapshot
            NavTreeUtils.ExpandSnapShotItems(SnapShot, treeRootItem);
        }
        private void LoadImagesToListbox(object obj = null)
        { }
        // Constructors
        public NavTreeVm(int pRootNumber = 0, bool pIncludeFileChildren = true)
        {
            // create a new RootItem given rootNumber using convention
            RootNr = pRootNumber;
             treeRootItem = NavTreeRootItemUtils.ReturnRootItem(pRootNumber, pIncludeFileChildren);
            TreeName = treeRootItem.FriendlyName;
            //treeRootItem.IsExpanded
            // Delete RootChildren and init RootChildren using treeRootItem.Children
            foreach (INavTreeItem item in RootChildren) { item.DeleteChildren(); }
            RootChildren.Clear();

            foreach (INavTreeItem item in treeRootItem.Children)
            { RootChildren.Add(item); }
            selectedPathFromTreeCommand = new DelegateCommand(LoadImagesToListbox);
        }

        // Well I suppose with the implicit values these are just for the record/illustration  
        public NavTreeVm(int rootNumber) : this(rootNumber, false) 
        {  }
        public NavTreeVm()
            : this(0)
        {
            
        }
    }
}



