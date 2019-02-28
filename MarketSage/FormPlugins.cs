using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using MarketSage.Library;

/* MarketSage
   Copyright © 2008, 2009 Jeffrey Morton
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

namespace MarketSage
{
    public partial class FormPlugins : Form
    {
        private PluginServices plugin = new PluginServices();
        AvailablePlugin<IIndicator> selected;

        public FormPlugins()
        {
            InitializeComponent();
        }

        private void FormPlugins_Load(object sender, EventArgs e)
        {
            plugin.FindPlugins(Properties.Settings.Default.GlobalDirectoryRoot + Properties.Settings.Default.GlobalDirectoryPlugins);
            foreach (AvailablePlugin<IIndicator> plug in plugin.AvailablePlugins)
            {
                Assembly pluginAssembly = Assembly.LoadFrom(plug.AssemblyPath);
                plug.Instance = (IIndicator)Activator.CreateInstance(pluginAssembly.GetType(plug.PlugType.ToString()));
                IIndicator rne = plug.Instance;
                TreeNode node = new TreeNode(rne.ToString());
                node.Tag = plug;
                treePlugins.Nodes.Add(node);
            }
            if (treePlugins.Nodes.Count > 0)
            {
                selected = (AvailablePlugin<IIndicator>)treePlugins.Nodes[0].Tag;
                lblName.Text = selected.Instance.Name;
                txtAuthor.Text = selected.Instance.Author;
                txtDescription.Text = selected.Instance.Description;
                txtVersion.Text = selected.Instance.Version;
            }
        }

        private void treePlugins_AfterSelect(object sender, TreeViewEventArgs e)
        {
            selected = (AvailablePlugin<IIndicator>)e.Node.Tag;
            lblName.Text = selected.Instance.Name;
            txtAuthor.Text = selected.Instance.Author;
            txtDescription.Text = selected.Instance.Description;
            txtVersion.Text = selected.Instance.Version;
        }
    }
}