/*
 * Tilt-Compensated eCompass Function.
 * The function calls the trigonometric functions iTrig and iHundredAtan2Deg.
 * The three angles computed should be low-pass filtered (see next section).
 * Global variables used by the function are listed immediately below.
 */

/* roll pitch and yaw angles computed by iecompass */
static Int16 iPhi, iThe, iPsi;

/* magnetic field readings corrected for hard iron effects and PCB orientation */
static Int16 iBfx, iBfy, iBfz;

/* hard iron estimate */
static Int16 iVx, iVy, iVz;

/* tilt-compensated e-Compass code */
public static void iecompass( Int16 iBpx, Int16 iBpy, Int16 iBpz,
                              Int16 iGpx, Int16 iGpy, Int16 iGpz )
{
    /* stack variables */
    /* iBpx, iBpy, iBpz: the three components of the magnetometer sensor */
    /* iGpx, iGpy, iGpz: the three components of the accelerometer sensor */
    /* local variables */
    Int16 iSin, iCos; /* sine and cosine */

    /* subtract the hard iron offset */
    iBpx -= iVx; /* see Eq 16 */
    iBpy -= iVy; /* see Eq 16 */
    iBpz -= iVz; /* see Eq 16 */

    /* calculate current roll angle Phi */
    iPhi = iHundredAtan2Deg(iGpy, iGpz);/* Eq 13 */

    /* calculate sin and cosine of roll angle Phi */
    iSin = iTrig(iGpy, iGpz); /* Eq 13: sin = opposite / hypotenuse */
    iCos = iTrig(iGpz, iGpy); /* Eq 13: cos = adjacent / hypotenuse */

    /* de-rotate by roll angle Phi */
    iBfy = (Int16)((iBpy * iCos - iBpz * iSin) >> 15);/* Eq 19 y component */
    iBpz = (Int16)((iBpy * iSin + iBpz * iCos) >> 15);/* Bpy*sin(Phi)+Bpz*cos(Phi)*/
    iGpz = (Int16)((iGpy * iSin + iGpz * iCos) >> 15);/* Eq 15 denominator */

    /* calculate current pitch angle Theta */
    iThe = iHundredAtan2Deg((Int16)-iGpx, iGpz);/* Eq 15 */

    /* restrict pitch angle to range -90 to 90 degrees */
    if (iThe > 9000) iThe = (Int16) (18000 - iThe);
    if (iThe < -9000) iThe = (Int16) (-18000 - iThe);

    /* calculate sin and cosine of pitch angle Theta */
    iSin = (Int16)-iTrig(iGpx, iGpz); /* Eq 15: sin = opposite / hypotenuse */
    iCos = iTrig(iGpz, iGpx); /* Eq 15: cos = adjacent / hypotenuse */

    /* correct cosine if pitch not in range -90 to 90 degrees */
    if (iCos < 0) iCos = (Int16)-iCos;

    /* de-rotate by pitch angle Theta */
    iBfx = (Int16)((iBpx * iCos + iBpz * iSin) >> 15); /* Eq 19: x component */
    iBfz = (Int16)((-iBpx * iSin + iBpz * iCos) >> 15);/* Eq 19: z component */

    /* calculate current yaw = e-compass angle Psi */
    iPsi = iHundredAtan2Deg((Int16)-iBfy, iBfx); /* Eq 22 */
}

Int32 tmpAngle;      /* temporary angle*100 deg: range -36000 to 36000 */
static Int16 iLPPsi; /* low pass filtered angle*100 deg: range -18000 to 18000 */

static UInt16 ANGLE_LPF; /* low pass filter: set to 32768 / N for N samples averaging */

/* implement a modulo arithmetic exponential low pass filter on the yaw angle */
/* compute the change in angle modulo 360 degrees */
tmpAngle = (Int32)iPsi - (Int32)iLPPsi;
if (tmpAngle > 18000) tmpAngle -= 36000;
if (tmpAngle < -18000) tmpAngle += 36000;

/* calculate the new low pass filtered angle */
tmpAngle = (Int32)iLPPsi + ((ANGLE_LPF * tmpAngle) >> 15);

/* check that the angle remains in -180 to 180 deg bounds */
if (tmpAngle > 18000) tmpAngle -= 36000;
if (tmpAngle < -18000) tmpAngle += 36000;

/* store the correctly bounded low pass filtered angle */
iLPPsi = (Int16)tmpAngle;

if (tmpAngle > 9000) tmpAngle = (Int16) (18000 - tmpAngle);
if (tmpAngle < -9000) tmpAngle = (Int16) (-18000 - tmpAngle);
