using System.Windows.Media.Imaging;
using System;
using System.Collections.ObjectModel;
using System.IO;
using APPBASE;
using Modelss;
using System.Collections.Generic;


namespace SUVI_APP.ViewModels.TreeviewViewModel.TreeviewClass
{
    // Definition of several NavTreeItem classes (constructor, GetMyIcion and GetMyChildren) from abstact class NavTreeItem 
    // Note that this file can be split/refactored in smaller parts

    // RootItems
    // - Special items are "RootItems" such as DriveRootItem with as children DriveItems
    //   other RootItems might be DriveNoChildRootItem, FavoritesRootItem, SpecialFolderRootItem, 
    //   (to do) LibraryRootItem, NetworkRootItem, HistoryRootItem.
    // - We use RootItem(s) as a RootNode for trees, their Children (for example DriveItems) are copied to RootChildren VM
    // - Binding in View: TreeView.ItemsSource="{Binding Path=NavTreeVm.RootChildren}"
    
    // DriveRootItem has DriveItems as children 
    public class DriveRootItem : NavTreeItem
    {
        public DriveRootItem()
        {
            //Constructor sets some properties
            FriendlyName = "DriveRoot";
            IsExpanded = true;
            FullPathName = "$xxDriveRoot$";
        }

        public override BitmapSource GetMyIcon()
        {
            // Note: introduce more "speaking" icons for RootItems
            string Param = "pack://application:,,,/" + "MyImages/bullet_blue.png";
            Uri uri1 = new Uri(Param, UriKind.RelativeOrAbsolute);
            return myIcon = BitmapFrame.Create(uri1);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            INavTreeItem item1;
            string fn = "";

            //string[] allDrives = System.Environment.GetLogicalDrives();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo drive in allDrives)
                if (drive.IsReady)
                {
                    item1 = new DriveItem();

                    // Some processing for the FriendlyName
                    fn = drive.Name.Replace(@"\", "");
                    item1.FullPathName = fn;
                    if (drive.VolumeLabel == string.Empty)
                    {
                        fn = drive.DriveType.ToString() + " (" + fn + ")";
                    }
                    else if (drive.DriveType == DriveType.CDRom)
                    {
                        fn = drive.DriveType.ToString() + " " + drive.VolumeLabel + " (" + fn + ")";
                    }
                    else
                    {
                        fn = drive.VolumeLabel + " (" + fn + ")";
                    }

                    item1.FriendlyName = fn;
                    item1.IncludeFileChildren = this.IncludeFileChildren;
                    item1.BuildTreeCommand = new AppBase.DelegateCommand(Dclick);
                    childrenList.Add(item1);
                }

            return childrenList;
        }

        private void Dclick(object obj)
        {
          //  (obj as NavTreeItem).IsExpanded = (obj as NavTreeItem).IsExpanded ? false : true;

            //  throw new NotImplementedException();
        }
    }

    // DriveNoChildRootItem has DriveNoChildItems as children
    public class DriveNoChildRootItem : NavTreeItem
    {
        public DriveNoChildRootItem()
        {
            //Constructor sets some properties
            FriendlyName = "DrivesRoot";
            IsExpanded = true;
            FullPathName = "$xxDriveRoot$";
        }

        public override BitmapSource GetMyIcon()
        {
            string Param = "pack://application:,,,/" + "MyImages/bullet_blue.png";
            Uri uri1 = new Uri(Param, UriKind.RelativeOrAbsolute);
            return myIcon = BitmapFrame.Create(uri1);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            INavTreeItem item1;
            string fn = "";

            //string[] allDrives = System.Environment.GetLogicalDrives();
            DriveInfo[] allDrives = DriveInfo.GetDrives(); //GetLogicalDrives();
            foreach (DriveInfo drive in allDrives)
                if (drive.IsReady)
                {
                    item1 = new DriveNoChildItem();

                    // Some processing for the FriendlyName
                    fn = drive.Name.Replace(@"\", "");
                    item1.FullPathName = fn;
                    if (drive.VolumeLabel == string.Empty)
                    {
                        fn = drive.DriveType.ToString() + " (" + fn + ")";
                    }
                    else if (drive.DriveType == DriveType.CDRom)
                    {
                        fn = drive.DriveType.ToString() + " " + drive.VolumeLabel + " (" + fn + ")";
                    }
                    else
                    {
                        fn = drive.VolumeLabel + " (" + fn + ")";
                    }

                    item1.FriendlyName = fn;
                    item1.IncludeFileChildren = true;
                    childrenList.Add(item1);
                }

            return childrenList;
        }
    }


    // FavoritesRootItem has Windows7 "File Explorer" Favorites as children, will not work on non windows 7 systems
    // Does not work quite properly, cannot find documentation. Did some hacking but order of items unkown 
    // If you don't have windows7 remove/rename this class/constructor ...Root_Item 
    // Or choose your own folder see (**) and fill it with drive/folder shortcuts
    public class FavoritesRootItem : NavTreeItem
    {
        public FavoritesRootItem()
        {
            FriendlyName = "Favorites"; // tmp hack: fixed Name
            FullPathName = "$xxFavoritesRoot$";
            IsExpanded = true;
        }

        public override BitmapSource GetMyIcon()
        {
            // to do: nice icon for this ItemRoots
            string Param = "pack://application:,,,/" + "MyImages/bullet_blue.png";
            Uri uri1 = new Uri(Param, UriKind.RelativeOrAbsolute);
            return myIcon = BitmapFrame.Create(uri1);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            INavTreeItem item1;
            string fn = "";

            // This does not work yet properly: know the folder, note also desktop.ini present
            // 1) Localisation of name and path 
            // 2) How is the order specified, cannot find documentation

            // If not Windows7, no children. I cannot test this now.
            if (!TestCurrentOs.IsWindows7()) return childrenList;

            // tmp hack, fixed filename. 
            // (**) Non Windows 7: Specify fn= your own favorites folder and put some Drive/Folder shortcuts in this folder      
            Environment.SpecialFolder s = Environment.SpecialFolder.UserProfile;
            fn = Environment.GetFolderPath(s);
            fn = fn + "\\Links";  

            try
            {
                // For favorites we always return files!!
                DirectoryInfo di = new DirectoryInfo(fn);
                if (!di.Exists) return childrenList;

                string fileResolvedShortCut;

                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name.ToUpper().EndsWith(".LNK"))
                    {
                        // tmp hack: resolve link to display icons instead of link-icons
                        fileResolvedShortCut = FolderPlaneUtils.ResolveShortCut(file.FullName);
                        if (fileResolvedShortCut != "")
                        {
                            FileInfo fileNs = new FileInfo(fileResolvedShortCut);

                            // to do localisation, names??
                            item1 = new FileItem();
                            item1.FullPathName = fileNs.FullName;
                            item1.FriendlyName = (fileNs.Name != "") ? fileNs.Name : fileNs.ToString();

                            childrenList.Add(item1);
                        }
                    }
                }
            }
            catch
            {

            }
            return childrenList;
        }
    }

    // SpecialFolderRootItem has SpecialFolders as Children. Not so usefull, tempory addition to have some RootItems
    // For compatability windows XP use in this example SpecialFolders instead of KnownFolders 
    public class SpecialFolderRootItem : NavTreeItem
    {
        public SpecialFolderRootItem()
        {
            FriendlyName = "SpecialFolderRoot";
            FullPathName = "$xxSpecialFolderRoot$";
        }

        public override BitmapSource GetMyIcon()
        {
            string Param = "pack://application:,,,/" + "MyImages/bullet_blue.png";
            Uri uri1 = new Uri(Param, UriKind.RelativeOrAbsolute);
            return myIcon = BitmapFrame.Create(uri1);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            INavTreeItem item1;
            string fn = "";

            // If not Windows7, no children? 
            // I use specialFolders instead of KnownFolders for comaptability of older OS, to do: test if this works  
            // if (!Utils.TestCurrentOs.IsWindows7()) return childrenList;

            // We show all items, incl. hidden

            var allSpecialFoldersV = Enum.GetValues(typeof(System.Environment.SpecialFolder));
            foreach (Environment.SpecialFolder s in allSpecialFoldersV)
            {
                fn = Environment.GetFolderPath(s);
                if (fn != string.Empty)
                {
                    item1 = new FolderItem();
                    item1.FullPathName = fn;
                    item1.FriendlyName = s.ToString();
                    item1.IncludeFileChildren = true;
                    item1.IncludeFileChildren = this.IncludeFileChildren;
                    childrenList.Add(item1);
                }

            }

            return childrenList;
        }
    }

    // DriveItem has Folders and Files as children
    public class DriveItem : NavTreeItem
    {
        INavTreeItem item1;
        public override BitmapSource GetMyIcon()
        {
            //string Param = "pack://application:,,,/" + "MyImages/diskdrive.png";
            //Uri uri1 = new Uri(Param, UriKind.RelativeOrAbsolute);
            //return myIcon = BitmapFrame.Create(uri1);
            return myIcon = GetIconFn.GetIconDll(this.FullPathName);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
          

            DriveInfo drive = new DriveInfo(this.FullPathName);
            if (!drive.IsReady) return childrenList;

            DirectoryInfo di = new DirectoryInfo(((DriveInfo)drive).RootDirectory.Name);
            if (!di.Exists) return childrenList;

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                item1 = new FolderItem();
                item1.FullPathName = FullPathName + "\\" + dir.Name;
                item1.FriendlyName = dir.Name;
                
                item1.BuildTreeCommand = new AppBase.DelegateCommand(click);
                item1.IncludeFileChildren = true;// this.IncludeFileChildren;
                childrenList.Add(item1);
                item1.test = item1;
            }

            if (this.IncludeFileChildren)
            {
                foreach (FileInfo file in di.GetFiles())
                {
                    item1 = new FileItem();
                    item1.FullPathName = FullPathName + "\\" + file.Name;
                    item1.FriendlyName = file.Name;
                    childrenList.Add(item1);
                }
            }
            return childrenList;
        }
        public static List<T> RemoveDuplicatesSet<T>(List<T> items)
        {
            // Use HashSet to remember items seen.
            var result = new List<T>();
            var set = new HashSet<T>();
            for (int i = 0; i < items.Count; i++)
            {
                // Add if needed.
                if (!set.Contains(items[i]))
                {
                    result.Add(items[i]);
                    set.Add(items[i]);
                }
            }
            return result;
        }

        private void click(object obj)
        {
            try
            {
                (obj as NavTreeItem).IsExpanded = (obj as NavTreeItem).IsExpanded ? false : true;

                string[] test = Directory.GetFiles((obj as NavTreeItem).FullPathName, "*.jpg");
                for (int i = 0; i < test.Length; i++)
                {
                    AddingImageList.ImagePaathS.Add(test[i]);
                }
                AddingImageList.ImagePaathS = RemoveDuplicatesSet(AddingImageList.ImagePaathS);
                AddingImageList.BingingImagessBingingImagess.Clear();
                AddingImageList.ImagePaathS.ForEach(w => { AddingImageList.BingingImagessBingingImagess.Add(new ListBoxImageModel() { imgHieght =50,imgWidth =50, AspectRatio = AddingImageList.ImageRatio(w),  ListBoxImageNo = AddingImageList.BingingImagessBingingImagess.Count,  ImageData = AddingImageList.LoadImage(w),Title = w }); });
          
            }
            catch (Exception)
            {


            }
        }
    }

    // DriveItem has no children, is never expanded. Somewaht usefull to start from unexpanded drives//tempory addition of RootItems
    public class DriveNoChildItem : NavTreeItem
    {
        public override BitmapSource GetMyIcon()
        {
            return myIcon = GetIconFn.GetIconDll(this.FullPathName);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            return childrenList;
        }
    }

    public class FolderItem : NavTreeItem
    {
        public override BitmapSource GetMyIcon()
        {
            return myIcon = GetIconFn.GetIconDll(this.FullPathName);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            INavTreeItem item1;

            try
            {
                DirectoryInfo di = new DirectoryInfo(this.FullPathName); // may be acces not allowed
                if (!di.Exists) return childrenList;
                foreach (DirectoryInfo dir in di.GetDirectories())
                {
                    item1 = new FolderItem();
                    item1.FullPathName = FullPathName + "\\" + dir.Name;
                    item1.BuildTreeCommand = new AppBase.DelegateCommand(click);
                    item1.FriendlyName = dir.Name;
                    item1.IncludeFileChildren = true;
                    childrenList.Add(item1);
                    item1.test = item1;
                }

                if (this.IncludeFileChildren) foreach (FileInfo file in di.GetFiles())
                    {
                        item1 = new FileItem();
                        item1.FullPathName = FullPathName + "\\" + file.Name;
                        item1.BuildTreeCommand = new AppBase.DelegateCommand(click);
                        item1.CheckForImages = file.Name.Contains(".jpg");
                        item1.FriendlyName = file.Name;
                       
                        childrenList.Add(item1);
                    }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
            }
            return childrenList;
        }


        public static List<T> RemoveDuplicatesSet<T>(List<T> items)
        {
            // Use HashSet to remember items seen.
            var result = new List<T>();
            var set = new HashSet<T>();
            for (int i = 0; i < items.Count; i++)
            {
                // Add if needed.
                if (!set.Contains(items[i]))
                {
                    result.Add(items[i]);
                    set.Add(items[i]);
                }
            }
            return result;
        }

        private void click(object obj)
        {
            try
            {
                (obj as NavTreeItem).IsExpanded = (obj as NavTreeItem).IsExpanded ? false : true;

                string[] test = Directory.GetFiles((obj as NavTreeItem).FullPathName, "*.jpg");
                for (int i = 0; i < test.Length; i++)
                {
                    AddingImageList.ImagePaathS.Add(test[i]);
                }
              AddingImageList.ImagePaathS=  RemoveDuplicatesSet(AddingImageList.ImagePaathS);
              AddingImageList.BingingImagessBingingImagess.Clear();
              AddingImageList.ImagePaathS.ForEach(w => { AddingImageList.BingingImagessBingingImagess.Add(new ListBoxImageModel() { imgHieght = 50, imgWidth = 50, ListBoxImageNo = AddingImageList.BingingImagessBingingImagess.Count, ImageData = AddingImageList.LoadImage(w), Title = w }); });
               
            }
            catch (Exception)
            {
                
               
            }
           
        }
    }

    public class FileItem : NavTreeItem
    {
        public override BitmapSource GetMyIcon()
        {
            // to do, use a cache for .ext != "" or ".exe" or ".lnk"
            return myIcon = GetIconFn.GetIconDll(this.FullPathName);
        }

        public override ObservableCollection<INavTreeItem> GetMyChildren()
        {
            ObservableCollection<INavTreeItem> childrenList = new ObservableCollection<INavTreeItem>() { };
            return childrenList;
        }
    }
}
