using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace DailyWork
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }


        #region Show Filters

        private bool _high = true;
        public bool High
        {
            get { return _high; }
            set 
            { 
                _high = value; 
                RaisePropertyChanged("High"); 
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _low = true;
        public bool Low
        {
            get { return _low; }
            set 
            { 
                _low = value; 
                RaisePropertyChanged("Low");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _normal = true;
        public bool Normal
        {
            get { return _normal; }
            set 
            { 
                _normal = value; 
                RaisePropertyChanged("Normal");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }


        private bool _open = true;
        public bool Open
        {
            get { return _open; }
            set 
            { 
                _open = value; 
                RaisePropertyChanged("Open");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _inProgress = true;
        public bool InProgress
        {
            get { return _inProgress; }
            set 
            { 
                _inProgress = value; 
                RaisePropertyChanged("InProgress");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _waiting = true;
        public bool Waiting
        {
            get { return _waiting; }
            set 
            { 
                _waiting = value; 
                RaisePropertyChanged("Waiting");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _completed = false;
        public bool Completed
        {
            get { return _completed; }
            set 
            { 
                _completed = value; 
                RaisePropertyChanged("Completed");
                ApplyFilters(this.Data);

                if (value == false)
                    All = false;
            }
        }

        private bool _all = true;
        public bool All
        {
            get { return _all; }
            set
            {
                _all = value;
                RaisePropertyChanged("All");

                if (_all)
                {
                    if (Low == false)
                        Low = true;

                    if (Normal == false)
                        Normal = true;

                    if (High == false)
                        High = true;

                    if (Open == false)
                        Open = true;

                    if (InProgress == false)
                        InProgress = true;

                    if (Waiting == false)
                        Waiting = true;

                    if (Completed == false)
                        Completed = true;
                }

                //ApplyFilters(this.Data);
            }
        }
        #endregion

        private ObservableCollection<Item> _statuses;
        public ObservableCollection<Item> Statuses
        {
            get { return _statuses; }
            set { _statuses = value; RaisePropertyChanged("Statuses"); }
        }

        private ObservableCollection<Item> _priorities;
        public ObservableCollection<Item> Priorities
        {
            get { return _priorities; }
            set { _priorities = value; RaisePropertyChanged("Priorities"); }
        }

        private ObservableCollection<Item> _teamMembers;
        public ObservableCollection<Item> TeamMembers
        {
            get { return _teamMembers; }
            set { _teamMembers = value; RaisePropertyChanged("TeamMembers"); }
        }



        private string _task;
        public string Task
        {
            get { return _task; }
            set { _task = value; RaisePropertyChanged("Task"); }
        }

        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set { _date = value; RaisePropertyChanged("Date"); }
        }



        private Daily _selectedItem;
        public Daily SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged("SelectedItem");

                if (_selectedItem != null)
                {
                    this.Task = _selectedItem.Task;
                    this.Date = _selectedItem.Date;
                }
            }
        }


        private ObservableCollection<Daily> _data;
        public ObservableCollection<Daily> Data
        {
            get { return _data; }
            set { _data = value; RaisePropertyChanged("Data"); }
        }


        private ObservableCollection<Daily> _dataDisplay;
        public ObservableCollection<Daily> DataDisplay
        {
            get { return _dataDisplay; }
            set { _dataDisplay = value; RaisePropertyChanged("DataDisplay"); }
        }


        public RelayCommand AddCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
        public RelayCommand RemoveCommand { get; set; }

        public string FileName { get; set; }

        public MainWindowViewModel()
        {
            Statuses = new ObservableCollection<Item>();
            Statuses.Add(new Item { ID = "1", Name = "Open" });
            Statuses.Add(new Item { ID = "2", Name = "In Progress" });
            Statuses.Add(new Item { ID = "3", Name = "Waiting" });
            Statuses.Add(new Item { ID = "4", Name = "Completed" });

            Priorities = new ObservableCollection<Item>();
            Priorities.Add(new Item { ID = "1", Name = "High" });
            Priorities.Add(new Item { ID = "2", Name = "Normal" });
            Priorities.Add(new Item { ID = "3", Name = "Low" });

            TeamMembers = new ObservableCollection<Item>();
            TeamMembers = GetTeamMembers();
            //TeamMembers.Add(new Item { ID = "nataraju.janapalli@hexagonsi.com", Name = "JANAPALLI Nataraju" });
            //TeamMembers.Add(new Item { ID = "srinivas.kandru@hexagonsi.com", Name = "KANDRU Srinivas M R S (KMRS)" });
            //TeamMembers.Add(new Item { ID = "rajendraprasad.telugu@hexagonsi.com", Name = "TELUGU Rajendra Prasad G" });
            //TeamMembers.Add(new Item { ID = "sarfaraz.killedar@hexagonsi.com", Name = "KILLEDAR Sarfaraz S" });
            //TeamMembers.Add(new Item { ID = "kimberly.floyd@hexagonsi.com", Name = "FLOYD Kimberly R (Kim)" });
            //TeamMembers.Add(new Item { ID = "Krishnakant.Gudla@hexagonsi.com", Name = "GUDLA Krishnakant" });
            //TeamMembers.Add(new Item { ID = "saikumar.ravula@hexagonsi.com", Name = "RAVULA Sai Kumar" });
            //TeamMembers.Add(new Item { ID = "ravi.jatoth@hexagonsi.com", Name = "JATOTH Ravi" });
            //TeamMembers.Add(new Item { ID = "Dileep.Tangudu@hexagonsi.com", Name = "TANGUDU Dileep" });
            //TeamMembers.Add(new Item { ID = "pushpa.gorapalli@hexagonsi.com", Name = "GORAPALLI Pushpa" });
            //TeamMembers.Add(new Item { ID = "rahul.walke@hexagonsi.com", Name = "WALKE Rahul" });
            //
            //TeamMembers.Add(new Item { ID = "tanya.fussinger@hexagonsi.com", Name = "FUSSINGER Tanya K" });
            //TeamMembers.Add(new Item { ID = "eric.richardson@hexagonsi.com", Name = "RICHARDSON Eric T" });
            //TeamMembers.Add(new Item { ID = "jason.leister@hexagonsi.com", Name = "LEISTER Jason A" });
            //TeamMembers.Add(new Item { ID = "ethan.taylor@hexagonsi.com", Name = "TAYLOR Ethan P" });
            TeamMembers.Add(new Item { ID = "", Name = "" });

            AddCommand = new RelayCommand(Add);
            SaveCommand = new RelayCommand(Save);
            RemoveCommand = new RelayCommand(Remove);

            Data = new ObservableCollection<Daily>();

            FileName = ConfigurationManager.AppSettings["InputFile"].ToString();
            if (string.IsNullOrWhiteSpace(FileName))
                FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{DateTime.Now.Year}.xml");

            if (!File.Exists(FileName))
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<ArrayOfDaily xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
                sb.AppendLine("</ArrayOfDaily>");

                File.WriteAllText(FileName, sb.ToString());
            }

            this.Data = DeserializeObject(FileName);

            Init(this.Data);

            ApplyFilters(this.Data);
        }

        private ObservableCollection<Item> GetTeamMembers()
        {
            ObservableCollection<Item> result = new ObservableCollection<Item>();

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TeamMembers.txt");
            string[] lines = File.ReadAllLines(filePath);

            if(lines != null && lines.Count() > 0)
            {
                foreach(string line in lines)
                {
                    var split = line.Split(',');

                    if (!string.IsNullOrWhiteSpace(split[1]))
                        result.Add(new Item { ID = split[0], Name = split[1] });
                }
            }

            return result;
        }

        public ObservableCollection<Daily> DeserializeObject(string filename)
        {
            ObservableCollection<Daily> data = new ObservableCollection<Daily>();
            XmlSerializer serializer = new XmlSerializer(typeof(ObservableCollection<Daily>));

            using (Stream reader = new FileStream(filename, FileMode.Open))
            {
                data = (ObservableCollection<Daily>)serializer.Deserialize(reader);
            }

            return data;
        }

        public void Add()
        {
            Daily d = null;

            //if (this.SelectedItem != null)
            //{

            var t = this.Data.Where(r => string.IsNullOrWhiteSpace(r.Task)).FirstOrDefault();
            if (t != null)
            {
                d = t;
            }
            else
            {
                d = new Daily();
                d.ID = Convert.ToInt64(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
                d.Date = DateTime.Now;
                d.Priority = "2";
                d.Status = "1";
                d.CreatedOn = DateTime.Now;

                this.Data.Add(d);
            }

            //}
            //else
            //{
            //    d = new Daily();
            //    d.Date = DateTime.Now;
            //    d.Priority = "2";
            //    d.Status = "1";
            //}

            if (d != null)
                this.SelectedItem = d;

            ApplyFilters(this.Data);
        }

        public void Save()
        {
            //if (string.IsNullOrWhiteSpace(this.Task))
            //    return;
            //
            //if (this.Data.Where(r => r.Task.ToUpper().Trim().Equals(this.Task.ToUpper().Trim())).Count() == 0)
            //{
            //    this.Data.Add(new Daily { Task = this.Task, Status="1", Date = this.Date, Priority="1" });
            //
            //    this.Task = string.Empty;
            //    this.Date = DateTime.Now;
            //}
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ObservableCollection<Daily>));
                serializer.Serialize(writer, this.Data);
                writer.Flush();
            }

            ApplyFilters(this.Data);
        }

        public void Remove()
        {
            if (this.SelectedItem == null)
                return;

            this.Data.Remove(this.SelectedItem);
            this.SelectedItem = null;

            ApplyFilters(this.Data);
        }

        public void Init(ObservableCollection<Daily> data)
        {
            if (data == null)
                return;

            var rows = data.Where(r => r.ID == 0);
            foreach (var row in rows)
            {
                if (row.Date != null)
                    row.ID = Convert.ToInt64(row.Date.ToString("yyyyMMddHHmmssfff"));
            }

            //var rows1 = data.Where(row => row.Priority.Equals("1") || row.Priority.Equals("2") || row.Priority.Equals("3") || row.Status.Equals("1") || row.Status.Equals("2") || row.Status.Equals("3") || row.Status.Equals("4"));
            //foreach (var row in rows1)
            //{
            //    if (row.Priority.Equals("1") || row.Priority.Equals("2") || row.Priority.Equals("3"))
            //    {
            //        var t = Priorities.Where(r => r.ID.Equals(row.Priority)).FirstOrDefault();
            //        if (t != null)
            //            row.Priority = t.Name;
            //    }
            //
            //    if (row.Status.Equals("1") || row.Status.Equals("2") || row.Status.Equals("3") || row.Status.Equals("4"))
            //    {
            //        var t = Statuses.Where(r => r.ID.Equals(row.Status)).FirstOrDefault();
            //        if (t != null)
            //            row.Priority = t.Name;
            //    }
            //}
        }

        public ObservableCollection<Daily> ApplyFilters(ObservableCollection<Daily> data)
        {
            if (data == null)
                new ObservableCollection<Daily>();

            ObservableCollection<Daily> result = new ObservableCollection<Daily>();

            try
            {
                var temp = data.Where(r => (r.Priority.Equals(this.High ? "1" : "-1") || r.Priority.Equals(this.Normal ? "2" : "-1") || r.Priority.Equals(this.Low ? "3" : "-1")));
                temp = temp.Where(r => (r.Status.Equals(this.Open ? "1" : "-1") || r.Status.Equals(this.InProgress ? "2" : "-1") || r.Status.Equals(this.Waiting ? "3" : "-1") || r.Status.Equals(this.Completed ? "4" : "-1"))).OrderByDescending(r=>r.Date);

                result = new ObservableCollection<Daily>(temp);

                this.DataDisplay = result;

                this.Status = $"Records Count : {this.DataDisplay?.Count}";
            }
            catch (Exception ex)
            {
                result = this.DataDisplay = data;
            }

            return result;
        }
    }

    public class Daily : ViewModelBase
    {
        public long ID { get; set; }

        private DateTime date;
        public DateTime Date
        {
            get { return date; }
            set { date = value; RaisePropertyChanged("Date"); }
        }

        private string _priority;
        public string Priority
        {
            get { return _priority; }
            set { _priority = value; RaisePropertyChanged("Priority"); }
        }


        private string _status;
        public string Status
        {
            get { return _status; }
            set { _status = value; RaisePropertyChanged("Status"); }
        }

        private string _teamMember_From;
        public string TeamMember_From
        {
            get { return _teamMember_From; }
            set { _teamMember_From = value; RaisePropertyChanged("TeamMember_From"); }
        }

        private string _teamMember_To;
        public string TeamMember_To
        {
            get { return _teamMember_To; }
            set { _teamMember_To = value; RaisePropertyChanged("TeamMember_To"); }
        }


        //public string Category { get; set; }

        private string _task;
        public string Task
        {
            get { return _task; }
            set { _task = value; RaisePropertyChanged("Task"); }
        }

        private string _details;
        public string Details
        {
            get { return _details; }
            set { _details = value; RaisePropertyChanged("Details"); }
        }


        private DateTime _createdOn;
        public DateTime CreatedOn
        {
            get { return _createdOn; }
            set { _createdOn = value; RaisePropertyChanged("CreatedOn"); }
        }


    }


    public class Item
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
