
/*
 * The function iHundredAtanDeg computes the function for X and Y in the range 0 to 32767
 * (interpreted as 0.0 to 0.9999695 in Q15 fractional arithmetic)
 * outputting the angle in degrees * 100 in the range 0 to 9000 (0.0° to 90.0°).
 *
 * calculates 100*atan(iy/ix) range 0 to 9000 for all ix, iy positive in range 0 to 32767
 */

 /* fifth order of polynomial approximation giving 0.05 deg max error */
 const Int16 K1 = 5701;
 const Int16 K2 = -1645;
 const Int16 K3 = 446;

 static Int16 iHundredAtanDeg(Int16 iy, Int16 ix)
 {

    Int32 iAngle; /* angle in degrees times 100 */
    Int16 iRatio; /* ratio of iy / ix or vice versa */
    Int32 iTmp; /* temporary variable */

    /* check for pathological cases */
    if ((ix == 0) && (iy == 0)) return (0);
    if ((ix == 0) && (iy != 0)) return (9000);

    /* check for non-pathological cases */
    if (iy <= ix)
       iRatio = iDivide(iy, ix); /* return a fraction in range 0. to 32767 = 0. to 1. */
    else
       iRatio = iDivide(ix, iy); /* return a fraction in range 0. to 32767 = 0. to 1. */

    /* first, third and fifth order polynomial approximation */
    iAngle = (Int32) K1 * (Int32) iRatio;
    iTmp = ((Int32) iRatio >> 5) * ((Int32) iRatio >> 5) * ((Int32) iRatio >> 5);
    iAngle += (iTmp >> 15) * (Int32) K2;
    iTmp = (iTmp >> 20) * ((Int32) iRatio >> 5) * ((Int32) iRatio >> 5)
    iAngle += (iTmp >> 15) * (Int32) K3;
    iAngle = iAngle >> 15;

    /* check if above 45 degrees */
    if (iy > ix) iAngle = (Int16)(9000 - iAngle);

    /* for tidiness, limit result to range 0 to 9000 equals 0.0 to 90.0 degrees */
    if (iAngle < 0) iAngle = 0;
    if (iAngle > 9000) iAngle = 9000;

    return ((Int16) iAngle);
 }