namespace ArchyManager.Classes.Archy2014
{
    class Archy2014DB
    {
        public ARCHS[] ARCHSs { get; set; }
        public ArchSite[] ArchSites { get; set; }
        public ArchSitePhotolog[] ArchSitePhotologs { get; set; }
        //public ARCHSPolygon[] ARCHSPolygons { get; set; }
        public AreaGeometry[] AreaGeometries { get; set; }
        public CMT[] CMTs { get; set; }
        public CMTMark[] CMTMarks { get; set; }
        public CrewDefinition[] CrewDefinitions { get; set; }
        public Datum[] Datums { get; set; }
        public HSF[] HSFs { get; set; }
        public ProfileDescriptor[] ProfileDescriptors { get; set; }
        public Project[] Projects { get; set; }
        public ShovelTestPit[] ShovelTestPits { get; set; }
        public STPProfile[] STPProfiles { get; set; }
        public TestPitCrew[] TestPitCrews { get; set; }
        public TrackLog[] Tracklogs { get; set; }
        public WCA[] WCAs { get; set; }
    }
    public enum ArchyRegion
    {
        BC = 0,
        NB = 1
    }
}




//class Archy2014View
//{
//    public Archy2014View(Archy2014DB db, ArchyRegion region)
//    {
//        if (region == ArchyRegion.BC)
//        {
//            // should also be verified
//            ArchSites = new ObservableCollection<ArchSite>(db.ArchSites);
//            ArchSitePhotologs = new ObservableCollection<ArchSitePhotolog>(db.ArchSitePhotologs);
//            AreaGeometries = new ObservableCollection<AreaGeometry>(db.AreaGeometries);
//            CMTs = new ObservableCollection<CMT>(db.CMTs);
//            CMTMarks = new ObservableCollection<CMTMark>(db.CMTMarks);
//            CrewDefinitions = new ObservableCollection<CrewDefinition>(db.CrewDefinitions);
//            Datums = new ObservableCollection<Datum>(db.Datums);
//            HSFs = new ObservableCollection<HSF>(db.HSFs);
//            ProfileDescriptors = new ObservableCollection<ProfileDescriptor>(db.ProfileDescriptors);
//            Projects = new ObservableCollection<Project>(db.Projects);
//            ShovelTestPits = new ObservableCollection<ShovelTestPit>(db.ShovelTestPits);
//            STPProfiles = new ObservableCollection<STPProfile>(db.STPProfiles);
//            TestPitCrews = new ObservableCollection<TestPitCrew>(db.TestPitCrews);
//            Tracklogs = new ObservableCollection<TrackLog>(db.Tracklogs);
//        }
//        else if (region == ArchyRegion.NB)
//        {
//            // need to verify these before running the next 
//            ARCHSs = new ObservableCollection<ARCHS>(db.ARCHSs);
//            ARCHSPolygons = new ObservableCollection<ARCHSPolygon>(db.ARCHSPolygons);
//            AreaGeometries = new ObservableCollection<AreaGeometry>(db.AreaGeometries);
//            HSFs = new ObservableCollection<HSF>(db.HSFs);
//            Projects = new ObservableCollection<Project>(db.Projects);
//            ProfileDescriptors = new ObservableCollection<ProfileDescriptor>(db.ProfileDescriptors);
//            ShovelTestPits = new ObservableCollection<ShovelTestPit>(db.ShovelTestPits);
//            STPProfiles = new ObservableCollection<STPProfile>(db.STPProfiles);
//            Tracklogs = new ObservableCollection<TrackLog>(db.Tracklogs);
//            WCAs = new ObservableCollection<WCA>(db.WCAs);
//        }
//    }

//    public ObservableCollection<ARCHS> ARCHSs { get; set; }
//    public ObservableCollection<ArchSite> ArchSites { get; set; }
//    public ObservableCollection<ArchSitePhotolog> ArchSitePhotologs { get; set; }
//    public ObservableCollection<ARCHSPolygon> ARCHSPolygons { get; set; }
//    public ObservableCollection<AreaGeometry> AreaGeometries { get; set; }
//    public ObservableCollection<CMT> CMTs { get; set; }
//    public ObservableCollection<CMTMark> CMTMarks { get; set; }
//    public ObservableCollection<CrewDefinition> CrewDefinitions { get; set; }
//    public ObservableCollection<Datum> Datums { get; set; }
//    public ObservableCollection<HSF> HSFs { get; set; }
//    public ObservableCollection<ProfileDescriptor> ProfileDescriptors { get; set; }
//    public ObservableCollection<Project> Projects { get; set; }
//    public ObservableCollection<ShovelTestPit> ShovelTestPits { get; set; }
//    public ObservableCollection<STPProfile> STPProfiles { get; set; }
//    public ObservableCollection<TestPitCrew> TestPitCrews { get; set; }
//    public ObservableCollection<TrackLog> Tracklogs { get; set; }
//    public ObservableCollection<WCA> WCAs { get; set; }
//}