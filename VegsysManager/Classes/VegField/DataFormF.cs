using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class DataFormF
    {
        public static DataFormF Create(DataRow row)
        {
            Type dffType = typeof(DataFormF);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            DataFormF dataformf = new DataFormF();
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

        public Guid DataFormFGuid { get; set; }
        public Guid SiteGuid { get; set; }

        // Most of the vegetated part of the AA (wetland Assessment Area) is a (select ONE):
        public bool? F1_1 { get; set; }     //wooded swamp
        public bool? F1_2 { get; set; }     //bog
        public bool? F1_3 { get; set; }     //fen
        public bool? F1_4 { get; set; }     //marsh or shallow open water


        // Mark all other vegetated wetland types in the AA that occupy more than 1 hectare or more than 1% of the vegetated AA.  
        // Do not mark the predominant type again.  If AA is larger than 1 ha, see preliminary imagery-based answer provided by AESRD, but field-verify.
        public bool? F2_1 { get; set; }     //wooded swamp
        public bool? F2_2 { get; set; }     //bog
        public bool? F2_3 { get; set; }     //fen
        public bool? F2_4 { get; set; }     //marsh or shallow open water
        public bool? F2_5 { get; set; }     //no types other than the predominant one in F1 meet the area threshold


        // At mid-day during the warmest time of year, the area of surface water within the AA that is shaded 
        // (by emergent or woody vegetation, incised channels, streambanks, or other features also present within the AA) is:
        public bool? F5_1 { get; set; }     //<5% of the water is shaded, or no surface water is present then
        public bool? F5_2 { get; set; }     //5-25% of the water is shaded
        public bool? F5_3 { get; set; }     //25-50% of the water is shaded
        public bool? F5_4 { get; set; }     //50-75% of the water is shaded
        public bool? F5_5 { get; set; }     //>75% of the water is shaded


        // During most of the time when water is present, its depth in most of the area is: [Note: This is not asking for the maximum depth]
        public bool? F10_1 { get; set; }    //<10 cm deep (but >0)
        public bool? F10_2 { get; set; }    //10 - 50 cm deep
        public bool? F10_3 { get; set; }    //0.5 - 1 m deep
        public bool? F10_4 { get; set; }    //1 - 2 m deep
        public bool? F10_5 { get; set; }    //>2 m deep.  True for many fringe wetlands


        // When present, surface water in most of the AA usually consists of (select one):
        public bool? F11_1 { get; set; }    //One depth class that comprises >90% of the AA’s inundated area (use the classes in the question above)
        public bool? F11_2 { get; set; }    //One depth class that comprises 60-90% of the AA's inundated area
        public bool? F11_3 { get; set; }    //Neither of above.  Multiple depth classes; none occupy more than 50% of the AA


        // The percentage of the AA's surface water that is ponded (stagnant, or flows so slowly that fine sediment is not held in suspension) 
        // during most of the time it is present, and which is either open or shaded by emergent vegetation is:
        public bool? F12_1 { get; set; }    //<1% or none, or occupies <0.01 hectare cumulatively.  Nearly all water is flowing.  Enter "1" and SKIP to F21 (Stained Surface Water)
        public bool? F12_2 { get; set; }    //1-5% of the water, and mainly in small pools.  The rest is flowing
        public bool? F12_3 { get; set; }    //1-5% of the water, and mainly in a single large pool or pond.  The rest is flowing
        public bool? F12_4 { get; set; }    //5-30% of the water
        public bool? F12_5 { get; set; }    //30-70% of the water
        public bool? F12_6 { get; set; }    //70-95% of the water
        public bool? F12_7 { get; set; }    //>95% of the water.  Little or no visibly flowing water within the AA


        // In ducks-eye aerial view, the percentage of the ponded water that is open (lacking emergent vegetation during most of the growing season, 
        // and unhidden by a forest or shrub canopy) is:
        public bool? F14_1 { get; set; }    //<1% or none, or largest pool occupies <0.01 hectares.  Enter "1" and SKIP to F20 (Floating Algae & Duckweed)
        public bool? F14_2 { get; set; }    //1-5% of the ponded water.  Enter "1" and SKIP to F20
        public bool? F14_3 { get; set; }    //5-30% of the ponded water
        public bool? F14_4 { get; set; }    //30-70% of the ponded water
        public bool? F14_5 { get; set; }    //70-99% of the ponded water
        public bool? F14_6 { get; set; }    //100% of the ponded water


        // The length of the AA's shoreline (along its ponded open water) that is bordered by areas that are nearly flat (a slope less than about 5%) is:
        public bool? F15_1 { get; set; }    //<1%
        public bool? F15_2 { get; set; }    //1-25%
        public bool? F15_3 { get; set; }    //25-50%
        public bool? F15_4 { get; set; }    //50-75%
        public bool? F15_5 { get; set; }    //>75%


        // At the driest time of year (or lowest water level), the average width of vegetated area in the AA that separates adjoining 
        // uplands from open water within the AA is:
        public bool? F16_1 { get; set; }    //<1 m
        public bool? F16_2 { get; set; }    //1 - 9 m
        public bool? F16_3 { get; set; }    //10 - 29 m
        public bool? F16_4 { get; set; }    //30 - 49 m
        public bool? F16_5 { get; set; }    //50 - 100 m
        public bool? F16_6 { get; set; }    //> 100 m


        // Near waters that are deeper than 0.5 m, the cover for fish, aquatic invertebrates, and/or amphibians that is provided by 
        // horizontally incised banks and/or partly-submerged accumulations of wood thicker than 10 cm (NOT by living vegetation) is:
        public bool? F17_1 { get; set; }    //Little or none, or all water is shallower than 0.5 m most of the year
        public bool? F17_2 { get; set; }    //Intermediate
        public bool? F17_3 { get; set; }    //Extensive


        // During most of the growing season, the spatial pattern of tall robust herbaceous vegetation (e.g., cattail, bulrush) 
        // that has surface water beneath it is mostly:
        public bool? F18_1 { get; set; }    //Scattered in small clumps, islands, or patches throughout the surface water area
        public bool? F18_2 { get; set; }    //Intermediate
        public bool? F18_3 { get; set; }    //Clumped at one or a few sides of the surface water area, or mostly surrounds a central area of open water.  
                                            //Or such vegetation is absent or covers <9 sq m and <1% of the AA

        public bool? F19_1 { get; set; }    //The AA contains (or is part of) an island or beaver lodge within a lake, pond, or river, 
                                            //and is isolated from the shore by water depths >2 m on all sides during an average June. 
                                            //The island may be solid, or it may be a floating vegetation mat suitable for nesting waterbirds

        public bool? F20_1 { get; set; }    //At some time of the year, mats of algae and/or duckweed cover most of the AA's otherwise-unshaded 
                                            //water surface or blanket the underwater substrate.  If true, enter "1" in next column.  If untrue or uncertain, enter "0"

        public bool? F21_1 { get; set; }    //Most surface water is tea-colored (from tannins, not iron bacteria or silt), and/or its pH is usually <5.5. 
                                            //Nearby vegetation is mostly moss and/or conifers

        // Use of the AA by beaver during the past 5 years is (select most applicable ONE):
        public bool? F23_1 { get; set; }    //evident from direct observation or presence of gnawed limbs, dams, tracks, dens, lodges, or extensive stands of water-killed trees (snags).
        public bool? F23_2 { get; set; }    //likely based on known occurrence in the region and proximity to suitable habitat, which may include: (a) a persistent freshwater wetland, pond, or lake, or a perennial low or mid-gradient(<10%) channel, and(b) a corridor or multiple stands of hardwood trees and shrubs in vegetated areas near surface water.
        public bool? F23_3 { get; set; }    //unlikely because site characteristics above are deficient, and/or this is a settled area or other area where beaver are routinely removed.But beaver occur in this part of the region (i.e., within 25 km).
        public bool? F23_4 { get; set; }    //none. Beaver are absent from this part of the region


        // During its travel through the AA at the time of peak annual flow, most of the water arriving in channels [select only the ONE encountered by most of the incoming water]:
        public bool? F25_1 { get; set; }    //Does not bump into plant stems as it travels through the AA.  Nearly all the water continues to travel in unvegetated (often incised) channels that have little contact with wetland vegetation, or through a zone of open water such as an instream pond or lake
        public bool? F25_2 { get; set; }    //bumps into herbaceous vegetation but mostly remains in fairly straight channels
        public bool? F25_3 { get; set; }    //bumps into herbaceous vegetation and mostly spreads throughout, or is in widely meandering, multi-branched, or braided channels
        public bool? F25_4 { get; set; }    //bumps into tree trunks and/or shrub stems but mostly remains in fairly straight channels
        public bool? F25_5 { get; set; }    //bumps into tree trunks and/or shrub stems and follows a fairly indirect path from entrance to exit(meandering, multi-branched, or braided)


        // During major runoff events, in the places where surface water exits the AA or connected waters nearby, it:
        public bool? F27_1 { get; set; }    //mostly passes through a pipe, culvert, narrowly breached dike, berm, beaver dam, or other partial obstruction (other than natural topography) that does not appear to drain the wetland artificially during most of the growing season
        public bool? F27_2 { get; set; }    //leaves through natural exits(channels or diffuse outflow), not mainly through artificial or temporary features
        public bool? F27_3 { get; set; }    //exported more quickly than usual due to ditches or pipes within the AA(or connected to its outlet or within 10 m of the AA's edge) which drain the wetland artificially, or water is pumped out of the AA


        // Select first applicable choice. 
        public bool? F28_1 { get; set; }    //Groundwater monitoring has demonstrated that groundwater primarily discharges to the wetland for longer periods during the year than periods when the wetland recharges the groundwater.  Or, springs are known to be present within the AA
        public bool? F28_2 { get; set; }    //One or more of the following are true: 
                                                //(a) the upper end of the AA is located very close to the base of (but mostly not ON) a natural slope much steeper (usually >15%) than that within the AA and longer than 100 m, OR
                                                //(b) rust deposits(""iron floc""), colored precipitates, or dispersible natural oil sheen are prevalent in the AA, OR
                                                //(c) AA water is remarkably clear in contrast to naturally stained waters typical in nearby wetlands, OR
                                                //(d) AA is located at a geologic fault
        public bool? F28_3 { get; set; }    //Neither of above is true, although some groundwater may discharge to or flow through the AA. Or groundwater influx is unknown


        // The gradient along most of the flow path within the AA is:
        public bool? F29_1 { get; set; }    //<2%, or, no slope is ever apparent (i.e., flat). Or, the wetland is in a depression or pond with no inlet and no outlet.
        public bool? F29_2 { get; set; }    //2-5%
        public bool? F29_3 { get; set; }    //6-10%
        public bool? F29_4 { get; set; }    //>10%


        // Within the entire vegetated part of the AA, the percentage occupied by trees or shrubs taller than 1 m is:
        public bool? F30_1 { get; set; }    //<5% of the vegetated AA, or there is no woody vegetation in the AA.SKIP to F38 (N Fixers)
        public bool? F30_2 { get; set; }    //5-25%
        public bool? F30_3 { get; set; }    //25-50%
        public bool? F30_4 { get; set; }    //50-75%
        public bool? F30_5 { get; set; }    //>75%


        // The following best represents the distribution pattern of woody vegetation VS. unshaded herbaceous/ moss vegetation within the AA:
        public bool? F31_1 { get; set; }    //(a) Woody cover and herbaceous/ moss cover EACH comprise 30-70% of the vegetated part of the AA, AND (b) There are many patches of woody vegetation scattered widely within herbaceous/ moss vegetation, or many patches of herbaceous vegetation scattered widely within woody vegetation.
        public bool? F31_2 { get; set; }    //(a) Woody cover and herbaceous/ moss EACH comprise 30-70% of the vegetated AA, AND (b)There are few patches("islands") of woody vegetation scattered widely within herbaceous vegetation, or few patches of herbaceous / moss vegetation("gaps") scattered widely within woody vegetation.
        public bool? F31_3 { get; set; }    //(a) Woody cover OR herbaceous / moss comprise > 70 % of the vegetated AA, AND(b) There are several patches of the other scattered within it.
        public bool? F31_4 { get; set; }    //(a) Woody cover OR herbaceous / moss comprise > 70 % of the vegetated AA, AND(b) The other is absent or is mostly in a single area or distinct zone with almost no intermixing of woody and unshaded herbaceous / moss vegetation.


        // Within the vegetated part of the AA, just the woody plants taller than 3 m occupy:
        public bool? F32_1 { get; set; }    //<1% of the vegetated AA, or the AA lacks trees.  Enter "1" and SKIP to F35 (Exposed Shrub).
        public bool? F32_2 { get; set; }    //1-25% of the vegetated AA
        public bool? F32_3 { get; set; }    //25-50% of the vegetated AA
        public bool? F32_4 { get; set; }    //50-95% of the vegetated AA
        public bool? F32_5 { get; set; }    //>95% of the vegetated part of the AA


        // Mark all the classes of woody plants within the AA, but only IF they comprise more than 5% of the woody canopy within the AA.  
        // Do not count trees that adjoin but are not within the AA.
        public bool? F33_1 { get; set; }    //coniferous, 1-9 cm diameter and >1 m tall
        public bool? F33_2 { get; set; }    //broad-leaved deciduous 1-9 cm diameter and >1 m tall
        public bool? F33_3 { get; set; }    //coniferous, 10-19 cm diameter
        public bool? F33_4 { get; set; }    //broad-leaved deciduous 10-19 cm diameter
        public bool? F33_5 { get; set; }    //coniferous, 20-40 cm diameter
        public bool? F33_6 { get; set; }    //>broad-leaved deciduous 20-40 cm diameter
        public bool? F33_7 { get; set; }    //coniferous, >40 cm diameter
        public bool? F33_8 { get; set; }    //broad-leaved deciduous >40 cm diameter


        // The number of downed wood pieces longer than 2 m and with diameter >10 cm, and not persistently submerged, is:
        public bool? F34_1 { get; set; }    //Several ( >5 if AA is >5 hectares, less for smaller AAs)
        public bool? F34_2 { get; set; }    //Few or none


        // Woody vegetation 1 to 3 m tall that is not under the drip line of taller woody vegetation comprises:
        public bool? F35_1 { get; set; }    //<5% of the vegetated AA and (if a fringe wetland) <5% of its water edge.  Or <0.01 hectare.  SKIP to F38 (N Fixers)
        public bool? F35_2 { get; set; }    //5-25% of the vegetated AA or(if a fringe wetland) 5-25% of the water edge  -- whichever is greater
        public bool? F35_3 { get; set; }    //25-50% of the vegetated AA or the water edge, whichever is greater
        public bool? F35_4 { get; set; }    //50-95% of the vegetated AA or the water edge, whichever is greater
        public bool? F35_5 { get; set; }    //>95% of the vegetated part of the AA or the water edge, whichever is greater

        // Determine which two native shrub species (1 to 3 m tall) comprise the greatest portion of the native shrub cover
        // Then choose one of the following:
        public bool? F36_1 { get; set; }    //those species together comprise > 50% of the areal cover of native shrub species
        public bool? F36_2 { get; set; }    //those species together do not comprise > 50% of the areal cover of native shrub species


        // The percentage of the AA's tree or shrub cover that is broad-leaved deciduous and is taller than 1 meter is:
        public bool? F37_1 { get; set; }    //<1%, or largest patch occupies less than 0.01 hectare
        public bool? F37_2 { get; set; }    //1-25% of the tree or shrub cover(whichever has more)
        public bool? F37_3 { get; set; }    //25-50% of the tree or shrub cover(whichever has more)
        public bool? F37_4 { get; set; }    //50-75% of the tree or shrub cover(whichever has more)
        public bool? F37_5 { get; set; }    //>75% of the tree or shrub cover(whichever has more)


        // The percent of the AA's shrub plus ground cover that is nitrogen-fixing plants 
        // (e.g., alder, baltic (wire) rush, sweetgale, lupine, clover, other legumes) is:
        public bool? F38_1 { get; set; }    //<1% or none
        public bool? F38_2 { get; set; }    //1-25% of the shrub plus ground cover, in the AA or along its water edge(whichever has more)
        public bool? F38_3 { get; set; }    //25-50% of the shrub plus ground cover, in the AA or along its water edge(whichever has more)
        public bool? F38_4 { get; set; }    //50-75% of the shrub plus ground cover, in the AA or along its water edge(whichever has more)
        public bool? F38_5 { get; set; }    //>75% of the shrub plus ground cover, in the AA or along its water edge(whichever has more)



        // The number of large snags (diameter >20 cm) in the AA plus the upland area within 10 m of the wetland edge is:
        public bool? F39_1 { get; set; }    //Several ( >2/hectare) and a pond, lake, or slow-flowing water wider than 10 m is within 1 km.
        public bool? F39_2 { get; set; }    //Several( >2/hectare) but above not true.
        public bool? F39_3 { get; set; }    //Few or none


        // Within the part of the AA that lacks persistent surface water, the cover of moss is:
        public bool? F40_1 { get; set; }    //<5% of the ground cover
        public bool? F40_2 { get; set; }    //5-25% of the ground cover
        public bool? F40_3 { get; set; }    //25-50% of the ground cover
        public bool? F40_4 { get; set; }    //50-95% of the ground cover
        public bool? F40_5 { get; set; }    //>95% of the ground cover


        // Consider the parts of the AA that lack surface water at the driest time of the growing season.  
        // Viewed from directly above the ground layer, the predominant condition in those areas at that time is:
        public bool? F41_1 { get; set; }    //Little or no (<5%) bare ground is visible between erect stems or under canopy anywhere in the vegetated AA. Ground is extensively blanketed by dense thatch, moss, lichens, graminoids with great stem densities, or plants with ground-hugging foliage        
        public bool? F41_2 { get; set; }    //Slightly bare ground(5-20% bare between plants) is visible in places, but those areas comprise less than 5% of the unflooded parts of the AA
        public bool? F41_3 { get; set; }    //Much bare ground(20-50% bare between plants) is visible in places, and those areas comprise more than 5% of the unflooded parts of the AA
        public bool? F41_4 { get; set; }    //Other conditions
        public bool? F41_5 { get; set; }    //Not applicable.Surface water (either open or obscured by emergent plants) covers all of the AA all the time


        // Consider the parts of the AA that lack surface water at some time of the year.  Excluding slash from logging, the number of small pits, 
        // raised mounds, hummocks, boulders, upturned trees, animal burrows, gullies, natural levees, wide soil cracks, and microdepressions is:
        public bool? F42_1 { get; set; }    //Few or none (minimal microtopography; <1% of that area) 
        public bool? F42_2 { get; set; }    //Intermediate
        public bool? F42_3 { get; set; }    //Several(extensive micro-topography)


        // Within the AA, inclusions of upland that individually are >100 sq.m. are:
        public bool? F43_1 { get; set; }    //Few or none
        public bool? F43_2 { get; set; }    //Intermediate(1 - 10% of vegetated part of the AA)
        public bool? F43_3 { get; set; }    //Many(e.g., wetland-upland "mosaic", >10% of the vegetated AA)


        // In parts of the AA that lack persistent water, the texture of soil in the uppermost layer is mostly:  
        // [To determine this, use a trowel to check in at least 3 widely spaced locations, and use the soil texture key in Appendix A of the Manual]
        public bool? F44_1 { get; set; }    //Loamy: includes loam, sandy loam
        public bool? F44_2 { get; set; }    //Fines: includes silt, glacial flour, clay, clay loam, silty clay, silty clay loam, sandy clay, sandy clay loam
        public bool? F44_3 { get; set; }    //Organic
        public bool? F44_4 { get; set; }    //Coarse: includes sand, loamy sand, gravel, cobble, stones, boulders, fluvents, fluvaquents, riverwash


        // In parts of the AA that lack persistent water, the texture of soil in the uppermost layer is mostly:  
        // [To determine this, use a trowel to check in at least 3 widely spaced locations, and use the soil texture key in Appendix A of the Manual]
        public bool? F45_1 { get; set; }    //none, or <100 sq. m within the AA
        public bool? F45_2 { get; set; }    //100-1000 sq.m within the AA
        public bool? F45_3 { get; set; }    //1000 – 10,000 sq.m within the AA
        public bool? F45_4 { get; set; }    //>10,000 sq.m within the AA


        // In aerial ("ducks eye") view, the maximum annual cover of dense herbaceous vegetation (graminoids + forbs, but not mosses and submerged and floating aquatics) is:
        public bool? F46_1 { get; set; }    //<5% of the vegetated part of the AA (excluding parts that are moss-covered or beneath shrubs or trees), or <0.01 hectare (whichever is less).  Mark "1" here and SKIP to F50 (Invasive Plant Cover).
        public bool? F46_2 { get; set; }    //100-1000 sq.m within the AA
        public bool? F46_3 { get; set; }    //25-50% of the vegetated AA
        public bool? F46_4 { get; set; }    //50-95% of the vegetated AA
        public bool? F46_5 { get; set; }    //>95% of the vegetated AA


        // The areal cover of forbs reaches an annual maximum of:
        public bool? F47_1 { get; set; }    //<5% of the herbaceous & moss cover
        public bool? F47_2 { get; set; }    //5-25% of the herbaceous & moss cover
        public bool? F47_3 { get; set; }    //25-50% of the herbaceous & moss cover
        public bool? F47_4 { get; set; }    //50-95% of the herbaceous & moss cover
        public bool? F47_5 { get; set; }    //>95% of the herbaceous & moss cover.SKIP to F50 (Invasive Plant Cover)


        // Sedges (Carex spp.) and/or cottongrass (Eriophorum spp.) occupy:
        public bool? F48_1 { get; set; }    //<5% of the herbaceous cover, or <0.01 hectare
        public bool? F48_2 { get; set; }    //5-50% of the herbaceous cover
        public bool? F48_3 { get; set; }    //50-95% of the herbaceous cover
        public bool? F48_4 { get; set; }    //>95% of the herbaceous cover


        // Determine which two native herbaceous (forb and graminoid) species comprise the greatest portion of the herbaceous 
        // cover that is unshaded by a woody canopy.  Then choose one of the following:
        public bool? F49_1 { get; set; }    //those species together comprise > 50% of the areal cover of native herbaceous plants at any time during the year
        public bool? F49_2 { get; set; }    //those species together do not comprise > 50% of the areal cover of native herbaceous plants at any time during the year


        // In central Alberta, common invasive graminoids include smooth brome, most bluegrasses, quackgrass, timothy, alfalfa, 
        // reed canarygrass, red fescue, spreading bentgrass.  Common invasive forbs include most thistles and sow-thistles, most clovers, 
        // sweetclover, black medick, dandelion, great plantain, hemp-nettle, lamb's-quarters, shepherd's-purse, curly dock, pennycress, 
        // wallflower, hawksbeard, tansy, chickweed, sticky-willy bedstraw, stickseed, tall buttercup. Select first applicable choice:
        public bool? F50_1 { get; set; }    //invasive or other non-native species appear to be absent in the AA, or are present only in trace amount (a few individuals)
        public bool? F50_2 { get; set; }    //Invasive species are present in more than trace amounts, but comprise<5% of herbaceous cover (or woody cover, if the invasives are woody)
        public bool? F50_3 { get; set; }    //Invasive species comprise 5-20% of the herb cover
        public bool? F50_4 { get; set; }    //Invasive species comprise 20-50% of the herb cover
        public bool? F50_5 { get; set; }    //Invasive species comprise >50% of the herb cover


        // Along the wetland-upland boundary, the percent of the upland edge (within 3 m of wetland) that is 
        // occupied by plant species that are considered invasive (see above) is:
        public bool? F51_1 { get; set; }    //none of the upland edge (invasives apparently absent)
        public bool? F51_2 { get; set; }    //some(but<5%) of the upland edge
        public bool? F51_3 { get; set; }    //5-50% of the upland edge
        public bool? F51_4 { get; set; }    //most(>50%) of the upland edge


        // Along the wetland-upland edge and extending 30 m upslope, the percentage of the upland that contains 
        // natural (not necessarily native -- see column E) land cover taller than 10 cm is:
        public bool? F52_1 { get; set; }    //<5% 
        public bool? F52_2 { get; set; }    //5 to 30%
        public bool? F52_3 { get; set; }    //30 to 60%
        public bool? F52_4 { get; set; }    //60 to 90%
        public bool? F52_5 { get; set; }    //>90%.  SKIP to F54(Cliffs)


        // Within 30 m upslope of the wetland-upland edge closest to the AA, the upland land cover that is NOT 
        // unmanaged vegetation or water is mostly (mark ONE):
        public bool? F53_1 { get; set; }    //impervious surface, e.g., paved road, parking lot, building, exposed rock.
        public bool? F53_2 { get; set; }    //bare or nearly bare pervious surface or managed vegetation, e.g., lawn, annual crops, mostly-unvegetated clearcut, landslide, unpaved road, dike.



        public bool? F54_1 { get; set; }    //In the AA or within 100 m, there is a known salt lick, or elevated terrestrial features such as cliffs, 
                                            //talus slopes, stream banks, or excavated pits (but not riprap) that extend at least 2 m nearly vertically, 
                                            //are unvegetated, and potentially contain crevices or other substrate suitable for nesting or den areas.  Enter 1 (yes) or 0 (no).


        // The AA is (or is within, or contains) a "new" wetland resulting from human actions (e.g., excavation, impoundment) 
        // or debris flows,  or other factors affecting what once was upland (non-hydric) soil.
        public bool? F55_1 { get; set; }    //No
        public bool? F55_2 { get; set; }    //yes, and created 20 - 100 years ago
        public bool? F55_3 { get; set; }    //yes, and created 3-20 years ago
        public bool? F55_4 { get; set; }    //yes, and created within last 3 years
        public bool? F55_5 { get; set; }    //yes, but time of origin unknown
        public bool? F55_6 { get; set; }    //unknown if new within 20 years or not


        // From the best vantage point on public roads, public parking lots, public buildings, or well-defined public trails that 
        // intersect, adjoin, or are within 100 m of the wetland, some part of the wetland is (select best case):
        public bool? F56_1 { get; set; }    //easily visible
        public bool? F56_2 { get; set; }    //somewhat visible
        public bool? F56_3 { get; set; }    //barely or not visible


        // Assuming access permission was granted, select ALL statements that are true of the AA as it currently exists:
        public bool? F58_1 { get; set; }    //For an average person, walking is physically possible in (not just near) >5% of the AA during most of the growing season, e.g., free of deep water and dense shrub thickets.
        public bool? F58_2 { get; set; }    //Maintained roads, parking areas, or foot-trails are within 10 m of the AA, or the AA can be accessed part of the year by boats arriving via contiguous waters.
        public bool? F58_3 { get; set; }    //Within or near the AA, there is an interpretive center, trails with interpretive signs or brochures, and/or regular guided interpretive tours.
        public bool? F58_4 { get; set; }    //The AA contains or adjoins a public boat dock or ramp, or is within 1 km of a campground, picnic area, or winter sports park.


        public bool? F61_1 { get; set; }    //Boardwalks, paved trails, fences or other infrastructure and/or well-enforced regulations appear to 
                                            //effectively prevent visitors from walking on soils within nearly all of the AA when they are unfrozen.  Enter "1" if true.

        public bool? F62_1 { get; set; }    //Fences, observation blinds, platforms, paved trails, exclusion periods, and/or well-enforced prohibitions 
                                            //on motorized boats, off-leash pets, and off road vehicles appear to effectively exclude or divert visitors 
                                            //and their pets from the AA at critical times in order to minimize disturbance of wildlife (except during hunting seasons).  Enter "1" if true. 



        // Recent evidence was found within the AA of the following potentially-sustainable consumptive uses.  Select all that apply.
        public bool? F63_1 { get; set; }    //Low-impact commercial timber harvest (e.g., selective thinning)
        public bool? F63_2 { get; set; }    //Extraction of surface water without noticeably affecting surface water area, depth, or persistence.
        public bool? F63_3 { get; set; }    //Grazing by livestock
        public bool? F63_4 { get; set; }    //Harvesting of native plants, native hay, or mushrooms (observed or known, not assumed)
        public bool? F63_5 { get; set; }    //Hunting(observed or known, not assumed)
        public bool? F63_6 { get; set; }    //Furbearer trapping
        public bool? F63_7 { get; set; }    //Fishing(observed or known, not assumed)
        public bool? F63_8 { get; set; }    //No evidence of any of the above



        // Based on measurement from a surface water area larger than .01 hectare, the AA's surface water is mostly:
        public bool? F65_1 { get; set; }    //Brackish or saline (conductance of >25 mS/cm, or >5000 ppm TDS). Or plants that indicate saline conditions comprise >20% of ground cover.  Trees and shrubs mostly absent.  Salt crust obvious around the perimeter and on flats
        public bool? F65_2 { get; set; }    //Slightly brackish (conductance of 2.5- 25 mS/cm, or 500 - 5000 ppm TDS).  Or plants that indicate saline conditions comprise 1-20% of ground cover.  Salt crust may or may not be present along perimeter
        public bool? F65_3 { get; set; }    //Fresh(conductance of < 2.5 mS/cm, or<500 ppm TDS). Plants that indicate saline conditions are sparse or absent.No salt crust along perimeter
        public bool? F65_4 { get; set; }    //Unknown condition(was not measured because surface water absent or insufficient, or measurement conflicted with plant indicators)


        // If required, survey the AA for plant or animal species believed to be of conservation concern in Alberta (see list on Form F in Appendix A of Manual).  
        // Do so at appropriate times of the year, especially if the data review conducted during the office phase of this assessment indicated their past 
        // presence in the general vicinity.  If you do detect these species or have reliable knowledge of their recent (within ~5 years) occurrence within the 
        // AA, indicate that below.
        public bool? F69_1 { get; set; }    //One or more of the rare plant species was detected within the AA (Appendix A, Table A.4)
        public bool? F69_2 { get; set; }    //One or more of the rare fish species was detected within the AA
        public bool? F69_3 { get; set; }    //One or more of the rare amphibian species was detected within the AA
        public bool? F69_4 { get; set; }    //One or more of the rare waterbird species was detected within the AA
        public bool? F69_5 { get; set; }    //One or more of the rare songbird or mammal species was detected within the AA
        public bool? F69_6 { get; set; }    //None of the above, or no data

        // The AA's size (in hectares) was measured as the following (next column):
        public bool? F70_1 { get; set; }    // If this (or a corrected measurement) is GREATER than 10 hectares enter a "1" here:
    }
}
