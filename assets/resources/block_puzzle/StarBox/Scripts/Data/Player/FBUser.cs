using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// for Serializable Json

public class FBUser
{

    public string id;
    public string name;
    public Picture picture = new Picture();
    //public Friends friends = new Friends();

    public class Picture
    {
        public Data data = new Data();

        public class Data
        {
            public int height;
            public bool is_silhouette;
            public string url;
            public int width;
        }
    }

    public class Friends
    {
        public List<Data> data = new List<Data>();
        public Paging paging = new Paging();
        public Summary summary = new Summary();

        public class Data
        {
            public string name;
            public string id;
        }

        public class Paging
        {
            public Cursors cursors = new Cursors();

            public class Cursors
            {
                public string before;
                public string after;
            }
        }

        public class Summary
        {
            public int total_count;
        }
    }
}
