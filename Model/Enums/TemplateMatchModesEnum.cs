namespace Model.Enums
{
    //
    // Summary:
    //     Specifies the way the template must be compared with image regions
    //
    // Remarks:
    //     https://github.com/opencv/opencv/blob/d3bc563c6e01c2bc153f23e7393322a95c7d3974/modules/imgproc/include/opencv2/imgproc.hpp#L3672
    public enum TemplateMatchModesEnum
    {
        //
        // Summary:
        //     \f[R(x,y)= \sum _{x',y'} (T(x',y')-I(x+x',y+y'))^2\f]
        SqDiff,
        //
        // Summary:
        //     \f[R(x,y)= \frac{\sum_{x',y'} (T(x',y')-I(x+x',y+y'))^2}{\sqrt{\sum_{x',y'}T(x',y')^2
        //     \cdot \sum_{x',y'} I(x+x',y+y')^2}}\f]
        SqDiffNormed,
        //
        // Summary:
        //     \f[R(x,y)= \sum _{x',y'} (T(x',y') \cdot I(x+x',y+y'))\f]
        CCorr,
        //
        // Summary:
        //     \f[R(x,y)= \frac{\sum_{x',y'} (T(x',y') \cdot I(x+x',y+y'))}{\sqrt{\sum_{x',y'}T(x',y')^2
        //     \cdot \sum_{x',y'} I(x+x',y+y')^2}}\f]
        CCorrNormed,
        //
        // Summary:
        //     \f[R(x,y)= \sum _{x',y'} (T'(x',y') \cdot I'(x+x',y+y'))\f] where \f[\begin{array}{l}
        //     T'(x',y')=T(x',y') - 1/(w \cdot h) \cdot \sum _{x'',y''} T(x'',y'') \\ I'(x+x',y+y')=I(x+x',y+y')
        //     - 1/(w \cdot h) \cdot \sum _{x'',y''} I(x+x'',y+y'') \end{array}\f]
        CCoeff,
        //
        // Summary:
        //     \f[R(x,y)= \frac{ \sum_{x',y'} (T'(x',y') \cdot I'(x+x',y+y')) }{ \sqrt{\sum_{x',y'}T'(x',y')^2
        //     \cdot \sum_{x',y'} I'(x+x',y+y')^2} }\f]
        CCoeffNormed
    }
}