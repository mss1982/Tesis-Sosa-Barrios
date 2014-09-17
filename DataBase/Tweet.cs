﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace DataBaseSQL
{
    public class Tweet
    {
        string id;
        string tweet_id;
        string author;
        string entity_id;
        string category;
        string text;
        string id_category;

        public Tweet()
        {
            this.id = this.author = this.category = this.text = this.entity_id = this.tweet_id = this.id_category = string.Empty;
        }

        public string Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;
            }
        }

        public string Tweet_Id
        {
            get
            {
                return this.tweet_id;
            }
            set
            {
                this.tweet_id = value;
            }
        }

        public string Author
        {
            get
            {
                return this.author;
            }
            set
            {
                this.author = value;
            }
        }

        public string Entity_Id
        {
            get
            {
                return this.entity_id;
            }
            set
            {
                this.entity_id = value;
            }
        }

        public string Category
        {
            get
            {
                return this.category;
            }
            set
            {
                this.category = value;
            }
        }

        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        public string Id_Category
        {
            get
            {
                return this.id_category;
            }
            set
            {
                this.id_category = value;
            }
        }

    }
}