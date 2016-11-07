using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class DataFormF2
    {
        public static DataFormF2 Create(DataRow row)
        {
            Type dffType = typeof(DataFormF2);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            DataFormF2 dataformf = new DataFormF2();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    dataformf, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
            }
            return dataformf;
        } //creates a dataformf from an sqlce or (maybe also) sql database row

        public Guid DataFormF2Guid { get; set; }
        public Guid SiteGuid { get; set; }

        // The percentage of the AA that never contains surface water during an average year (that is, except perhaps 
        // for a few hours after snowmelt or rainstorms), but which is still a wetland, is:
        public bool? F3_1 { get; set; }     //less than 1%, or <0.01 hectare (about 10 m on a side) never has surface water.  In other words, all or nearly all of the AA is inundated permanently or at least seasonally.
        public bool? F3_2 { get; set; }     //1-25% of the AA never contains surface water
        public bool? F3_3 { get; set; }     //25-50% of the AA never contains surface water
        public bool? F3_4 { get; set; }     //50-99% of the AA never contains surface water
        public bool? F3_5 { get; set; }     //>99% of the AA never contains surface water, except for water flowing in channels and/or in pools that occupy<1% of the AA. SKIP to F26 (Channel Connection & Outflow Duration)
        public bool? F3_6 { get; set; }     //>99% of the AA never contains surface water, and AA is not intersected by channels that have flow, not even for a few days per year. SKIP to F26


        // The percentage of the AA that has surface water (either ponded or flowing, either open or obscured by vegetation) 
        // during all of the growing season during most years is:
        public bool? F4_1 { get; set; }     //less than 1%, or <0.01 hectare (whichever is less).  SKIP to F8 (% Flooded Only Seasonally)
        public bool? F4_2 { get; set; }     //1-25% of the AA, and mostly in narrow channels and/or small scattered pools
        public bool? F4_3 { get; set; }     //1-25% of the AA, and mostly in a single large pool, pond, and/or channel
        public bool? F4_4 { get; set; }     //25-50% of the AA
        public bool? F4_5 { get; set; }     //50-95% of the AA
        public bool? F4_6 { get; set; }     //>95% of the AA


        public bool? F6_1 { get; set; }     //If the AA meets the following conditions, it is a fringe wetland: (a) Open water that adjoins 
                                            //the vegetated wetland in a lake, stream, or river during annual low water condition is much wider 
                                            //than the vegetated wetland, and if the AA adjoins a lake (b) the maximum dimension of the lake is 
                                            //greater than 1 km. If true, enter "1" and continue


        public bool? F7_1 { get; set; }     //The AA borders a body of ponded open water whose size -- not counting the vegetated AA -- exceeds 
                                            //8 hectares (about 300 x 300 m) during most of the growing season.  Enter "1" if true, "0" if false.


        // The percentage of the AA that is covered by surface water only during the wettest time of year (and for >2 consecutive days during that time) is:
        public bool? F8_1 { get; set; }    //<1%. 
        public bool? F8_2 { get; set; }    //1-25% 
        public bool? F8_3 { get; set; }    //25-50% 
        public bool? F8_4 { get; set; }    //50-95% 
        public bool? F8_5 { get; set; }    //>95% 


        // The annual fluctuation in surface water level within most of the parts of the AA that contain surface water is:
        public bool? F9_1 { get; set; }    //<10 cm change (stable) 
        public bool? F9_2 { get; set; }    //10 cm - 50 cm change
        public bool? F9_3 { get; set; }    //0.5 - 1 m change
        public bool? F9_4 { get; set; }    //1-2 m change
        public bool? F9_5 { get; set; }    //>2 m change
        //public bool? F9_6 { get; set; }    // I don't see an F9_6 --------------------------------------------------------------------------------------------ERROR


        public bool? F13_1 { get; set; }    //During most of the growing season, the largest patch of open water that is ponded and is in or bordering 
                                            //the AA is >0.01 hectare (about 10 m by 10 m) and mostly deeper than 0.5 m.  If true enter "1" and continue,  
                                            //If false, enter "0" and SKIP to F20 (Floating Algae & Duckweed)


        public bool? F22_1 { get; set; }    //Fish (native or stocked) are known to be present in the AA.  Or fish can access at least part of the AA 
                                            //during one or more days annually.  If uncertain, use answer from Form OF, item  OF15, or enter "0"

        public bool? F24_1 { get; set; }    //At least once annually, surface water from a tributary >100m long, or from a larger permanent water body, 
                                            //moves into the AA. It may enter directly in a channel, or as unconfined overflow from a contiguous river or 
                                            //lake, or via a pipe or hardened conduit.   If true, enter 1 and continue.  If false, enter 0 and SKIP to F26 
                                            //(Channel Connection & Outflow Duration)


        // The most persistent surface water connection (outlet channel or pipe, ditch, or overbank water exchange) between the 
        // AA and the closest off-site downslope water body is:
        public bool? F26_1 { get; set; }    //persistent (>9 months/year, including times when frozen)
        public bool? F26_2 { get; set; }    //seasonal(14 days to 9 months/year, not necessarily consecutive, including times when frozen)
        public bool? F26_3 { get; set; }    //temporary(<14 days, not necessarily consecutive -- must be unfrozen)
        public bool? F26_4 { get; set; }    //none -- but maps show a stream or other water body that is downslope from the AA and within a distance that is less than the AA's length.  If so, mark "1" here and SKIP TO F28 (Groundwater). 
        public bool? F26_5 { get; set; }    //no surface water flows out of the wetland except possibly during extreme events(<once per 10 years). Or, water flows only into a wetland, ditch, or lake that lacks an outlet.If so, mark "1" here and SKIP TO F28(Groundwater). 


        // Most of the AA is (select one):
        public bool? F57_1 { get; set; }    //publicly owned conservation lands that exclude new timber harvest, roads, mineral extraction, and intensive summer recreation (e.g., off-road vehicles).  Includes most Protected Lands
        public bool? F57_2 { get; set; }    //publicly owned resource use lands(allowed activities such as timber harvest, mining, or intensive recreation), or unknown.Includes most Crown Reservations/Notations
        public bool? F57_3 { get; set; }    //Owned by non-profit conservation organization or lease holder who allows public access
        public bool? F57_4 { get; set; }    //Other private ownership, including First Nations


        // TThe percentage of the AA almost never visited by humans during an average growing season probably comprises: 
        // [Note: Do not include visitors on trails outside of the AA unless more than half the wetland is visible from the trails 
        // and they are within 30 m of the wetland edge.  In that case, imagine the percentage of the AA that would be covered by 
        // the trail if it were placed within the AA.]

        public bool? F59_1 { get; set; }    //<5% and no inhabited building is within 100 m of the AA
        public bool? F59_2 { get; set; }    //<5% and inhabited building is within 100 m of the AA
        public bool? F59_3 { get; set; }    //5-50% and no inhabited building is within 100 m of the AA
        public bool? F59_4 { get; set; }    //5-50% and inhabited building is within 100 m of the AA
        public bool? F59_5 { get; set; }    //50-95%
        public bool? F59_6{ get; set; }    //>95% of the AA


        // The percentage of the AA visited by humans almost daily for several weeks during an average 
        // growing season probably comprises: [Note: Do not include visitors on trails outside of the AA 
        // unless more than half the wetland is visible from the trails and they are within 30 m of the wetland edge.   
        // In that case, imagine the percentage of the AA that would be covered by the trail if it were placed within the AA.]
        public bool? F60_1 { get; set; }    //<5%.  If F59 was answered ">95%", SKIP to F63 (Consumptive Uses)
        public bool? F60_2 { get; set; }    //5-50%
        public bool? F60_3 { get; set; }    //50-95%
        public bool? F60_4 { get; set; }    //>95% of the AA


        // The closest wells or water bodies that currently provide drinking water are:
        public bool? F64_1 { get; set; }    //Within 100 m of the AA
        public bool? F64_2 { get; set; }    //100-500 m away
        public bool? F64_3 { get; set; }    //>500 m away, or no information


        // Sampling indicates problems with the quality of surface waters or sediment within the AA, or within 
        // 5 km upstream or upslope, as caused by (enter 1 for ALL that apply):

        public bool? F66_1 { get; set; }    //nutrients (phosphorus, nitrate, ammonia), or a water body within 5 km that contributes to the AA has been labeled "hyper-eutrophic" based on excessive levels of either total phosphorus or chlorophyll-a.
        public bool? F66_2 { get; set; }    //suspended sediment or turbidity
        public bool? F66_3 { get; set; }    //metals(mercury, lead, zinc, copper, cadmium, others)
        public bool? F66_4 { get; set; }    //petrochemicals(pesticides, herbicides, PCBs, others)
        public bool? F66_5 { get; set; }    //None of above, or no data


        // Mark ALL of the following that apply to this AA:
        public bool? F67_1 { get; set; }    //Regulatory Investment: The AA is all or part of a mitigation site used explicitly to offset impacts elsewhere
        public bool? F67_2 { get; set; }    //Non-regulatory Investment: The AA is part of or contiguous to a wetland on which public or private organizational funds were spent to preserve, create, restore, enhance, the wetland(excluding mitigation wetlands)
        public bool? F67_3 { get; set; }    //Sustained Scientific Use: Plants, animals, or water in the AA have been monitored for >2 years, unrelated to any regulatory requirements, and data are available to the public.  Or the AA is part of an area that has been designated by an agency or institution as a benchmark, reference, or status-trends monitoring area.
        public bool? F67_4 { get; set; }    //None of the above, or no information for any.


        // View the approximate boundaries of the wetland's catchment (CA) as shown in the map AESRD provides in response to your 
        // data request.  Then adjust those boundaries if necessary based on your field observations of the surrounding terrain, 
        // and/or by using procedures described in the ABWRET Manual. Relative to the extent of this catchment (but excluding the 
        // area of the AA), this AA and any bordering waters together comprise (select ONE):
        public bool? F68_1 { get; set; }    //<1% of their catchment
        public bool? F68_2 { get; set; }    //1 to 10% of their catchment
        public bool? F68_3 { get; set; }    //10 to 100%  of their catchment
        public bool? F68_4 { get; set; }    //Larger than the area of their catchment(wetland has essentially no catchment, e.g., isolated by dikes with no input channels, or is a raised bog)
    }
}
