
/*
 * Integer Division
 *
 * An accurate integer division function where it is given that both the numerator and denominator are non-negative,
 * non-zero and where the denominator is greater than the numerator.
 * The result is in the range 0 decimal to 32767 decimal
 * which is interpreted in Q15 fractional arithmetic as the range 0.0 to 0.9999695.
 *
 * Calculates:
 *    ir = iy / ix with iy <= ix, and ix, iy both > 0
 */

 const UInt16 MINDELTADIV = 1; /* final step size for iDivide */

 /* function to calculate ir = iy / ix with iy <= ix, and ix, iy both > 0 */
 static Int16 iDivide(Int16 iy, Int16 ix)
 {
    Int16 itmp; /* scratch */
    Int16 ir; /* result = iy / ix range 0., 1. returned in range 0 to 32767 */
    Int16 idelta; /* delta on candidate result dividing each stage by factor of 2 */

    /* set result r to zero and binary search step to 16384 = 0.5 */
    ir = 0;
    idelta = 16384; /* set as 2^14 = 0.5 */

    /* to reduce quantization effects, boost ix and iy to the maximum signed 16 bit value */
    while ((ix < 16384) && (iy < 16384))
    {
       ix = (Int16)(ix + ix);
       iy = (Int16)(iy + iy);
    }

    /* loop over binary sub-division algorithm solving for ir*ix = iy */
    do
    {
       /* generate new candidate solution for ir and test if we are too high or too low */
       itmp = (Int16)(ir + idelta); /* itmp=ir+delta, the candidate solution */
       itmp = (Int16)((itmp * ix) >> 15);
       if (itmp <= iy) ir += idelta;
       idelta = (Int16)(idelta >> 1); /* divide by 2 using right shift one bit */
    } while (idelta >= MINDELTADIV); /* last loop is performed for idelta=MINDELTADIV */

    return (ir);
 }