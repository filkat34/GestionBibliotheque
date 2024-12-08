﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GestionBibliotheque
{
    public partial class Main : Form
    {
        // création de la collection de livres
        private List<Livre> ListeLivres = new List<Livre>();

        // nom du fichier de sérialisation
        private String nomFic = "sauvLivres";

        public Main()
        {
            InitializeComponent();
            listBox1.SelectedIndex = -1;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            // positionner le combobox par défaut sur "Conserver"
            comboBoxStatut.SelectedIndex = 0;

            // récupérer la sauvegarde des contacts, si elle existe
            Object recupLivres = Serialise.Recup(nomFic);
            if (recupLivres != null)
            {
                ListeLivres = (List<Livre>)recupLivres;

                // remplir de la listbox avec les contacts récupérés
                MajListeLivres();
            }
        }

        private void MajListeLivres()
        {
            // sauvegarder la ligne sélectionnée de la listbox
            int currentIndex = listBox1.SelectedIndex;

            // trier la collection en comparant les noms des auteurs
            ListeLivres.Sort((auteur1, auteur2) => auteur1.GetAuteur().CompareTo(auteur2.GetAuteur()));

            // lier la ListBox avec ListeLivres pour la remplir
            BindingList<Livre> bdlLivres = new BindingList<Livre>(ListeLivres);
            listBox1.DataSource = bdlLivres;

            // sauvegarder la liste dans le fichier
            Serialise.Sauve(nomFic, ListeLivres);

            // compteur du nombre de livres dans la bibliothèque
            LblNumberofBooks.Text = "Livres : " + ListeLivres.Count.ToString();

            // compteur du nombre de livres à vendre dans la bibliothèque
            LblNbBooksToSell.Text = "A vendre : " + ListeLivres.Count(livre => livre.GetStatut() == "Vendre").ToString();

            // compteur du nombre de livres à donner dans la bibliothèque
            LblNbBooksToGive.Text = "A donner : " + ListeLivres.Count(livre => livre.GetStatut() == "Donner").ToString();

            // compteur du nombre de livres à recycler dans la bibliothèque
            LblNbBooksToRecycle.Text = "A recycler : " + ListeLivres.Count(livre => livre.GetStatut() == "Recycler").ToString();

            // forcer la listbox à rester sur la ligne sélectionnée après la mise à jour
            listBox1.SelectedIndex = currentIndex;

        }

        private void BtnAddBook_Click(object sender, EventArgs e)
        {
            // création d'un nouvel objet livre à ajouter
            string unTitre = textBoxTitre.Text;
            string unAuteur = textBoxAuteur.Text.ToUpper();
            string unStatut = comboBoxStatut.SelectedItem.ToString();
            Livre nouvLivre = new Livre(unTitre, unAuteur, unStatut);
            bool unDoublon = false;

            // vérification que le livre n'existe pas déjà dans la collection
            foreach(Livre livre in ListeLivres)
            {
                if (livre.GetAuteur().ToLower() == nouvLivre.GetAuteur().ToLower() && livre.GetTitre().ToLower() == nouvLivre.GetTitre().ToLower())
                {
                    unDoublon = true;
                }
            }

            // ajout du livre s'il ne s'agit pas d'un doublon
            if (unDoublon == false)
            {
                ListeLivres.Add(nouvLivre);
            }

            else
            {
                // demande à l'utilisateur s'il souhaite ajouter un doublon
                if (MessageBox.Show("Un livre du même auteur et ayant le même titre est déjà présent dans votre bibliothèque. Voulez-vous vraiment ajouter ce livre ?", "Confirmation", MessageBoxButtons.YesNo) == (DialogResult.Yes))
                {
                    ListeLivres.Add(nouvLivre);
                }
            }
            // vide le contenu des textbox
            textBoxTitre.Clear();
            textBoxAuteur.Clear();

            // mise à jour de la listbox
            MajListeLivres();
        }

        private void BtnDeleteBook_Click(object sender, EventArgs e)
        {
            // Supprime le livre sélectionné de la collection
            if (listBox1.SelectedIndex != -1)
            {
                if (MessageBox.Show("Voulez-vous vraiment supprimer ce livre ?", "Confirmation", MessageBoxButtons.YesNo) == (DialogResult.Yes))
                {
                    ListeLivres.RemoveAt(listBox1.SelectedIndex);
                    MajListeLivres();
                }
            }
        }

        private void BtnKeepBook_Click(object sender, EventArgs e)
        {
            // Change le statut du livre à "Conserver"
            if (listBox1.SelectedIndex != -1)
            { 
                Livre unLivre = ListeLivres[listBox1.SelectedIndex];
                unLivre.SetStatut("Conserver");
                MajListeLivres();
            }
               
        }

        private void BtnGiveBook_Click(object sender, EventArgs e)
        {
            // Change le statut du livre à "Donner"
            if (listBox1.SelectedIndex != -1)
            {
                Livre unLivre = ListeLivres[listBox1.SelectedIndex];
                unLivre.SetStatut("Donner");
                MajListeLivres();
            }
                
        }

        private void BtnBookSell_Click(object sender, EventArgs e)
        {
            // Change le statut du livre à "Vendre
            if (listBox1.SelectedIndex != -1)
            {
                Livre unLivre = ListeLivres[listBox1.SelectedIndex];
                unLivre.SetStatut("Vendre");
                MajListeLivres();
            }
        }

        private void BtnRecycleBook_Click(object sender, EventArgs e)
        {
            // Change le statut du livre à "Recycler"
            if (listBox1.SelectedIndex != -1)
            {
                Livre unLivre = ListeLivres[listBox1.SelectedIndex];
                unLivre.SetStatut("Recycler");
                MajListeLivres();
            }
        }

        private void BtnApplyBookModification_Click(object sender, EventArgs e)
        {
            // applique les modifications saisies sur le livre
            if (listBox1.SelectedIndex != -1)
            {
                string unTitre = textBoxTitre.Text;
                string unAuteur = textBoxAuteur.Text;
                string unStatut = comboBoxStatut.SelectedItem.ToString();
                Livre unLivre = ListeLivres[listBox1.SelectedIndex];
                unLivre.SetAuteur(textBoxAuteur.Text);
                unLivre.SetTitre(textBoxTitre.Text);
                unLivre.SetStatut(comboBoxStatut.Text);
                MajListeLivres();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // quand aucune ligne n'est sélectionnée, ne rien afficher
            if (listBox1.SelectedIndex == -1)
            {
                textBoxAuteur.Clear();
                textBoxTitre.Clear();
                comboBoxStatut.SelectedIndex = 0;
            }

            // quand une ligne est sélectionnée, remplir les textbox avec les informations du livre sélectionné à partir de ListeLivres
            if (listBox1.SelectedIndex != -1)
            {
                string currItem = listBox1.SelectedItem.ToString();
                foreach (Livre livre in ListeLivres)
                {
                    string str = livre.GetAuteur().ToString() + ". " + livre.GetTitre().ToString() + ' ' + '[' + livre.GetStatut().ToString() + ']';
                    if (currItem == str)
                    {
                        int index = ListeLivres.IndexOf(livre);
                        Livre unLivre = ListeLivres[index];
                        textBoxAuteur.Text = unLivre.GetAuteur().ToString();
                        textBoxTitre.Text = unLivre.GetTitre().ToString();
                        comboBoxStatut.Text = unLivre.GetStatut().ToString();
                    }

                }
            }
        }

        private void BtnClearTextboxes_Click(object sender, EventArgs e)
        {
            textBoxTitre.Clear();
            textBoxAuteur.Clear();
            comboBoxStatut.SelectedIndex = 0;
        }

        private void TxtBoxSearch_TextChanged(object sender, EventArgs e)
        {
            // quand textbox remplie boucler dans la collection pour trouver le livre recherché
            if (string.IsNullOrEmpty(TxtBoxSearch.Text) == false)
            {
                foreach (Livre livre in ListeLivres)
                {
                    string auteur = livre.GetAuteur().ToString();
                    string titre = livre.GetTitre().ToString();

                    if (auteur.ToLower().Contains(TxtBoxSearch.Text.ToLower()) | titre.ToLower().Contains(TxtBoxSearch.Text.ToLower()))
                    {
                        int index = ListeLivres.IndexOf(livre);
                        listBox1.SelectedIndex = index;
                    }
                }
            }

            if (TxtBoxSearch.Text == "")
            {
                MajListeLivres();
                listBox1.SelectedIndex = -1;
            }
        }

        private void BtnEraseSearch_Click(object sender, EventArgs e)
        {
            TxtBoxSearch.Clear();
        }

        private void BtnRTous_CheckedChanged(object sender, EventArgs e)
        {
            MajListeLivres();
        }

        private void BtnRDonner_CheckedChanged(object sender, EventArgs e)
        {
            if(BtnRDonner.Checked == true)
            {
                List<Livre> ListeLivresDonner = new List<Livre>();
                foreach (Livre livre in ListeLivres)
                {
                    if (livre.GetStatut() == "Donner")
                    {
                        ListeLivresDonner.Add(livre);
                    }
                }
                listBox1.DataSource = ListeLivresDonner;
            }
        }

     

        private void BtnRConserver_CheckedChanged(object sender, EventArgs e)
        {
            if (BtnRConserver.Checked == true)
            {
                List<Livre> ListeLivresConserver = new List<Livre>();
                foreach (Livre livre in ListeLivres)
                {
                    if (livre.GetStatut() == "Conserver")
                    {
                        ListeLivresConserver.Add(livre);
                    }
                }
                listBox1.DataSource = ListeLivresConserver;
            }
        }

        private void BtnRVendre_CheckedChanged(object sender, EventArgs e)
        {
            if (BtnRVendre.Checked == true)
            {
                List<Livre> ListeLivresVendre = new List<Livre>();
                foreach (Livre livre in ListeLivres)
                {
                    if (livre.GetStatut() == "Vendre")
                    {
                        ListeLivresVendre.Add(livre);
                    }
                }
                listBox1.DataSource = ListeLivresVendre;
            }
        }

        private void BtnRRecycler_CheckedChanged(object sender, EventArgs e)
        {
            if (BtnRRecycler.Checked == true)
            {
                List<Livre> ListeLivresRecycler = new List<Livre>();
                foreach (Livre livre in ListeLivres)
                {
                    if (livre.GetStatut() == "Recycler")
                    {
                        ListeLivresRecycler.Add(livre);
                    }
                }
                listBox1.DataSource = ListeLivresRecycler;
            }
        }
    }
}
