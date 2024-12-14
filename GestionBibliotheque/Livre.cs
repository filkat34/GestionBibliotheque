﻿using System;

namespace GestionBibliotheque
{
    [SerializableAttribute]
    public class Livre
    {
        //attributs
        private string titre;
        private string auteur;
        private string statut;

        //méthodes
        public Livre(string unTitre, string unAuteur, string unStatut)
        {
            titre = unTitre;
            auteur = unAuteur;
            statut = unStatut;
        }

        public override string ToString()
        {
            return auteur + ". " + titre + ' ' + '[' + statut + ']';
        }

        public string GetTitre()
        {
            return titre;
        }

        public string GetAuteur()
        {
            return auteur;
        }

        public string GetStatut() 
        {
           return statut;
        }

        public void SetTitre(string nouvTitre)
        {
            titre = nouvTitre;
        }

        public void SetAuteur(string nouvAuteur)
        {
            auteur = nouvAuteur;
        }

        public void SetStatut(string unStatut)
        {
            statut = unStatut;
        }
    }
}
