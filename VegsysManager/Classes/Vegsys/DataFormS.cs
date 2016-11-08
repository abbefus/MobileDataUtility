using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes.Vegsys
{
    class DataFormS
    {
        public static DataFormS Create(DataRow row)
        {
            Type dffType = typeof(DataFormS);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            DataFormS dataforms = new DataFormS();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    dataforms,
                    SqlCeConversion.CheckDBNull(row[column], t)
                );
            }
            return dataforms;
        } //creates a dataformf from an sqlce or (maybe also) sql database row

        public Int32 SiteID { get; set; }
        public Int32 DFSID { get; set; }

        // In the last column, place a check mark next to any item that is likely to have caused the timing of water inputs 
        // (but not necessarily their volume) to shift by hours, days, or weeks, becoming either more muted (smaller or less 
        // frequent peaks spread over longer times, more temporal homogeneity of flow or water levels) or more flashy (larger 
        // or more frequent spikes but over shorter times).  [FR, INV, PH, STR]			
        public bool? S5_1 { get; set; }     //flow regulation in tributaries or water level regulation in adjoining water body, or other control structure at water entry points that regulates inflow to the wetland			
        public bool? S5_2 { get; set; }     //snow storage areas that drain directly to the wetland
        public bool? S5_3 { get; set; }     //increased pavement and other impervious surface in the CA
        public bool? S5_4 { get; set; }     //straightening, ditching, dredging, and/or lining of tributary channels in the CA

        // If any items were checked above, then for each row of the table below, you may assign points.  However, if you 
        // believe the checked items had no measurable effect on the timing of water conditions in any part of the AA, then 
        // leave the "0's" for the scores in the following rows.  To estimate effects, contrast the current condition with 
        // the condition if the checked items never occurred or were no longer present. 			
        public byte? S5_5 { get; set; }     //Spatial extent within the wetland of timing shift 
        public byte? S5_6 { get; set; }     //When most of the timing shift began

        // Score the following 2 rows only if the altered inputs began within past 10 years, and only for the part of the wetland that experiences those.			
        public byte? S5_7 { get; set; }     //Input timing now vs. previously
        public byte? S5_8 { get; set; }     //Flashiness or muting


        // In the last column, place a check mark next to any item -- occurring in either the wetland or its CA -- that is 
        // likely to have accelerated the inputs of contaminants or salts to the AA.  [NRv, PRv, STR]			
        public bool? S6_1 { get; set; }     //stormwater or wastewater effluent (including failing septic systems), landfills, industrial facilities			
        public bool? S6_2 { get; set; }     //road salt
        public bool? S6_3 { get; set; }     //metals & chemical wastes from mining, shooting ranges, snow storage areas, oil/ gas extraction, other sources
        public bool? S6_4 { get; set; }     //oil or chemical spills(not just chronic inputs) from nearby roads
        public bool? S6_5 { get; set; }     //artificial drainage or erosion of contaminated or saline soils
        public bool? S6_6 { get; set; }     //pesticides, as applied to lawns, croplands, roadsides, or other areas in the CA

        // If any items were checked above, then for each row of the table below, you may assign points.  However, if you 
        // believe the checked items did not cumulatively expose the AA to significantly higher levels of contaminants 
        // and/or salts, then leave the "0's" for the scores in the following rows.  To estimate effects, contrast the 
        // current condition with the condition if the checked items never occurred or were no longer present. 			
        public byte? S6_7 { get; set; }     //Usual toxicity of most toxic contaminants
        public byte? S6_8 { get; set; }     //Frequency & duration of input
        public byte? S6_9 { get; set; }     //AA proximity to main sources(actual or potential)


        // In the last column, place a check mark next to any item -- occurring in either the wetland or its CA -- that is 
        // likely to have accelerated the inputs of nutrients to the wetland.  [STR]			
        public bool? S7_1 { get; set; }     //less than 1%, or <0.01 hectare (about 10 m on a side) never has surface water.  In other words, all or nearly all of the AA is inundated permanently or at least seasonally.
        public bool? S7_2 { get; set; }     //1-25% of the AA never contains surface water
        public bool? S7_3 { get; set; }     //25-50% of the AA never contains surface water
        public bool? S7_4 { get; set; }     //50-99% of the AA never contains surface water

        // If any items were checked above, then for each row of the table below, you may assign points.  However, if you 
        // believe the checked items did not cumulatively expose the AA to significantly more nutrients, then leave the "0's" 
        // for the scores in the following rows.  To estimate effects, contrast the current condition with the condition if 
        // the checked items never occurred or were no longer present. 			
        public byte? S7_5 { get; set; }     //Type of loading
        public byte? S7_6 { get; set; }     //Frequency & duration of input
        public byte? S7_7 { get; set; }     //AA proximity to main sources(actual or potential)


        // In the last column, place a check mark next to any item present in the CA that is likely to have elevated the load 
        // of waterborne or windborne sediment reaching the wetland from its CA.  [INV, SRv, STR]			
        public bool? S8_1 { get; set; }     //erosion from plowed fields, fill, timber harvest, dirt roads, vegetation clearing, fires			
        public bool? S8_2 { get; set; }     //erosion from construction, in-channel machinery in the CA
        public bool? S8_3 { get; set; }     //erosion from off-road vehicles in the CA
        public bool? S8_4 { get; set; }     //erosion from livestock or foot traffic in the CA
        public bool? S8_5 { get; set; }     //stormwater or wastewater effluent
        public bool? S8_6 { get; set; }     //sediment from road sanding, gravel mining, other mining, oil/ gas extraction
        public bool? S8_7 { get; set; }     //accelerated channel downcutting or headcutting of tributaries due to altered land use
        public bool? S8_8 { get; set; }     //other human-related disturbances within the CA

        // If any items were checked above, then for each row of the table below, you may assign points (3, 2, or 1 as shown 
        // in header) in the last column.  However, if you believe the checked items did not cumulatively add significantly 
        // more sediment or suspended solids to the AA, then leave the "0's" for the scores in the following rows.  To estimate 
        // effects, contrast the current condition with the condition if the checked items never occurred or were no longer present. 			
        public byte? S8_9 { get; set; }     //Erosion in CA
        public byte? S8_10 { get; set; }    //Recentness of significant soil disturbance in the CA
        public byte? S8_11 { get; set; }    //Duration of sediment inputs to the wetland
        public byte? S8_12 { get; set; }    //AA proximity to actual or potential sources


        // In the last column, place a check mark next to any item present in the wetland that is likely to have compacted, eroded, 
        // or otherwise altered the wetland's soil.  If the AA is a created or restored wetland or pond, exclude those actions.  
        // [CS, INV, NR, PH, STR]			
        public bool? S9_1 { get; set; }     //compaction from machinery, off-road vehicles, or mountain bikes, especially during wetter periods			
        public bool? S9_2 { get; set; }     //leveling or other grading not to the natural contour
        public bool? S9_3 { get; set; }     //tillage, plowing (but excluding disking for enhancement of native plants)	
        public bool? S9_4 { get; set; }     //fill or riprap, excluding small amounts of upland soils containing organic amendments(compost, etc.) or small amounts of topsoil imported from another wetland
        public bool? S9_5 { get; set; }     //excavation
        public bool? S9_6 { get; set; }     //ditch cleaning or dredging in or adjacent to the wetland
        public bool? S9_7 { get; set; }     //boat traffic in or adjacent to the wetland and sufficient to cause shore erosion or stir bottom sediments
        public bool? S9_8 { get; set; }     //artificial water level or flow manipulations sufficient to cause erosion or stir bottom sediments

        // If any items were checked above, then for each row of the table below, you may assign points.  However, if you believe 
        // the checked items did not measurably alter the soil structure and/or topography, then leave the "0's" for the scores in 
        // the following rows.  To estimate effects, contrast the current condition with the condition if the checked items never 
        // occurred or were no longer present. 			
        public byte? S9_9 { get; set; }     //Spatial extent of altered soil
        public byte? S9_10 { get; set; }    //Recentness of significant soil alteration in wetland
        public byte? S9_11 { get; set; }    //Duration
        public byte? S9_12 { get; set; }    //Timing of soil alteration
    }
}
