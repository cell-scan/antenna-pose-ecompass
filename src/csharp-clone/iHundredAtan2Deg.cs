/*
 * ATAN2 Calculation
 *   The function iHundredAtan2Deg is a wrapper function which implements the ATAN2 function by
 *   assigning the results of an ATAN function to the correct quadrant.
 *   The result is the angle in degrees times 100.
 *
 *   calculates 100*atan2(iy/ix)=100*atan2(iy,ix) in deg for ix, iy in range -32768 to 32767
 */

static Int16 iHundredAtan2Deg(Int16 iy, Int16 ix)
{
   Int16 iResult; /* angle in degrees times 100 */

   /* check for -32768 which is not handled correctly */
   if (ix == -32768) ix = -32767;
   if (iy == -32768) iy = -32767;

   /* check for quadrants */
   if ((ix >= 0) && (iy >= 0)) /* range 0 to 90 degrees */
      iResult = iHundredAtanDeg(iy, ix);
   else if ((ix <= 0) && (iy >= 0)) /* range 90 to 180 degrees */
      iResult = (Int16)(18000 - (Int16)iHundredAtanDeg(iy, (Int16)-ix));
   else if ((ix <= 0) && (iy <= 0)) /* range -180 to -90 degrees */
      iResult = (Int16)((Int16)-18000 + iHundredAtanDeg((Int16)-iy, (Int16)-ix));
   else /* ix >=0 and iy <= 0 giving range -90 to 0 degrees */
      iResult = (Int16)(-iHundredAtanDeg((Int16)-iy, ix));

   return (iResult);
}