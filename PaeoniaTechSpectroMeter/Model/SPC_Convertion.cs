using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PaeoniaTechSpectroMeter.Model
{
    public class SPC
    {
        #region properties
        public header_t header;
        public List<datablock_t> datablock;
        #endregion
        #region constructor
        public SPC()
        {
            header = new header_t(0);
            datablock = new List<datablock_t>();
        }
        #endregion
        public struct header_t
        {
            public byte fileType;
            public byte fileVersion;
            public byte typeCode;
            public byte exponentYVal;
            public int numPoints;
            public double firstX;
            public double lastX;
            public int numSubFiles;
            public byte XunitsTypeCode;
            public byte YunitsTypeCode;
            public byte ZunitsTypeCode;
            public byte postingDisposition;
            public int compressedDate;
            public byte[] resolutionDesc;
            public byte[] sourceIntrumentDes;
            public UInt16 peakPointNumberForInerferograms;
            public float[] spare;
            public byte[] memo;
            public byte[] xyzCustAxisStrings;
            public int byteOffsettoLogBlock;
            public int fileModificationFlag;
            public byte processingCode;
            public byte calibLvl;
            public UInt16 subMethodSampleInjectionNumber;
            public float floatingDataMultiplerConcentrationFactor;
            public byte[] methodFile;
            public float ZsubFileIncForEvenZMultifiles;
            public int numWPlanes;
            public float WplaneInc;
            public byte WaxisUnitsCode;
            public byte[] reserved;

            public header_t(int inNumPts)
            {
                fileType = 0;
                fileVersion = 0x4B;
                typeCode = 0;
                exponentYVal = 0x80; //80h = as floating point
                numPoints = inNumPts;
                firstX = 0;
                lastX = 0;
                numSubFiles = 0;
                XunitsTypeCode = 0;
                YunitsTypeCode = 0;
                ZunitsTypeCode = 0;
                postingDisposition = 0;
                compressedDate = 0;
                resolutionDesc = new byte[9];
                sourceIntrumentDes = new byte[9];
                peakPointNumberForInerferograms = 0;
                spare = new float[8];
                memo = new byte[130];
                xyzCustAxisStrings = new byte[30];
                byteOffsettoLogBlock = 0;
                fileModificationFlag = 0;
                processingCode = 0;
                calibLvl = 0;
                subMethodSampleInjectionNumber = 0;
                floatingDataMultiplerConcentrationFactor = 0;
                methodFile = new byte[48];
                ZsubFileIncForEvenZMultifiles = 0;
                numWPlanes = 0;
                WplaneInc = 0;
                WaxisUnitsCode = 0;
                reserved = new byte[187];
            }
        }

        public struct datablock_t
        {
            public subheader_t subheader;
            public List<Datapoint_t> dp;

            public datablock_t(int inNumPt)
            {
                dp = new List<Datapoint_t>();
                subheader = new subheader_t(inNumPt);
            }
        }

        public struct Datapoint_t
        {
            public float x;
            public float y;

            public Datapoint_t(float inX, float inY)
            {
                x = inX;
                y = inY;
            }
        }

        public struct subheader_t
        {
            public byte subFileFlags;
            public byte exponentYVal;
            public UInt16 subFileIndex;
            public float subFileStartZ;
            public float subFileEndZ;
            public float subFileNoise;
            public int numPoints;
            public int numCoAddedScans;
            public float WaxixVal;
            public char[] reserved;

            public subheader_t(int inNumPt)
            {
                subFileFlags = 0;
                exponentYVal = 0x80; //80h = as floating point
                subFileIndex = 0;
                subFileStartZ = 0;
                subFileEndZ = 0;
                subFileNoise = 0;
                numPoints = inNumPt;
                numCoAddedScans = 0;
                WaxixVal = 0;
                reserved = new char[4];
            }
        }

        public enum FILETYPE_FLG
        {
            BIT16 = 0,
            USE_EXPT_EXT = 2,
            MULTIFILE = 4,
            MULTIFILE_Z_RAND = 8,
            MULTIFILE_Z_ORDERLY = 16,
            CUST_AXIS_LABELS = 32,
            XY_MULTIFILE = 64,
            XY_FILE = 128
        }

        public enum FILEVERSION
        {
            NEW_FORMAT = 0x4B,
            OLD_LABCALC_FORMAT = 0x4D
        }

        #region methods
        public int getCompressDateTime(int year, int month, int day, int hour, int mins)
        {
            int compressedDate = 0;

            compressedDate = (year << 20) + (month << 16) + (day << 11) + (hour << 6) + mins;

            return compressedDate;
        }

        public byte[] getBytesArray(string axis, int size)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(axis);
            Array.Resize(ref bytes, size);

            return bytes;
        }

        public int exportSingleYFile(string filepath, List<datablock_t> data)
        {
            try
            {
                //write binary data
                using (BinaryWriter writer = new BinaryWriter(File.Open(SystemPath.GetLogPath +"\\" +DateTime.Now.ToString("yyyy-MMM") + "\\"+ filepath+".spc" , FileMode.Create,FileAccess.ReadWrite)))
                {
                    //write main header
                    writer.Write(this.header.fileType);
                    writer.Write(this.header.fileVersion);
                    writer.Write(this.header.typeCode);
                    writer.Write(this.header.exponentYVal);
                    writer.Write(this.header.numPoints);
                    writer.Write(this.header.firstX);
                    writer.Write(this.header.lastX);
                    writer.Write(this.header.numSubFiles);
                    writer.Write(this.header.XunitsTypeCode);
                    writer.Write(this.header.YunitsTypeCode);
                    writer.Write(this.header.ZunitsTypeCode);
                    writer.Write(this.header.postingDisposition);
                    writer.Write(this.header.compressedDate);
                    writer.Write(this.header.resolutionDesc);
                    writer.Write(this.header.sourceIntrumentDes);
                    writer.Write(this.header.peakPointNumberForInerferograms);
                    for (int n = 0; n < this.header.spare.Length; n++)
                        writer.Write(this.header.spare[n]);
                    writer.Write(this.header.memo);
                    writer.Write(this.header.xyzCustAxisStrings);
                    writer.Write(this.header.byteOffsettoLogBlock);
                    writer.Write(this.header.fileModificationFlag);
                    writer.Write(this.header.processingCode);
                    writer.Write(this.header.calibLvl);
                    writer.Write(this.header.subMethodSampleInjectionNumber);
                    writer.Write(this.header.floatingDataMultiplerConcentrationFactor);
                    writer.Write(this.header.methodFile);
                    writer.Write(this.header.ZsubFileIncForEvenZMultifiles);
                    writer.Write(this.header.numWPlanes);
                    writer.Write(this.header.WplaneInc);
                    writer.Write(this.header.WaxisUnitsCode);
                    writer.Write(this.header.reserved);

                    //write subheader
                    writer.Write(data[0].subheader.subFileFlags);
                    writer.Write(data[0].subheader.exponentYVal);
                    writer.Write(data[0].subheader.subFileIndex);
                    writer.Write(data[0].subheader.subFileStartZ);
                    writer.Write(data[0].subheader.subFileEndZ);
                    writer.Write(data[0].subheader.subFileNoise);
                    writer.Write(data[0].subheader.numPoints);
                    writer.Write(data[0].subheader.numCoAddedScans);
                    writer.Write(data[0].subheader.WaxixVal);
                    writer.Write(data[0].subheader.reserved);

                    //write Y 
                    for (int n = 0; n < this.datablock[0].dp.Count; n++)
                    {
                        writer.Write(this.datablock[0].dp[n].y);
                    }
                }

                return 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

            return 1;
        }

        public int exportSingleXYFile(string filepath, List<datablock_t> data)
        {
            try
            {
                //write binary data
                using (BinaryWriter writer = new BinaryWriter(File.Open(SystemPath.GetLogPath + "\\" + DateTime.Now.ToString("yyyy-MMM") + "\\" + filepath + ".spc", FileMode.Create, FileAccess.ReadWrite)))
                {
                    //write main header
                    writer.Write(this.header.fileType);
                    writer.Write(this.header.fileVersion);
                    writer.Write(this.header.typeCode);
                    writer.Write(this.header.exponentYVal);
                    writer.Write(this.header.numPoints);
                    writer.Write(this.header.firstX);
                    writer.Write(this.header.lastX);
                    writer.Write(this.header.numSubFiles);
                    writer.Write(this.header.XunitsTypeCode);
                    writer.Write(this.header.YunitsTypeCode);
                    writer.Write(this.header.ZunitsTypeCode);
                    writer.Write(this.header.postingDisposition);
                    writer.Write(this.header.compressedDate);
                    writer.Write(this.header.resolutionDesc);
                    writer.Write(this.header.sourceIntrumentDes);
                    writer.Write(this.header.peakPointNumberForInerferograms);
                    for (int n = 0; n < this.header.spare.Length; n++)
                        writer.Write(this.header.spare[n]);
                    writer.Write(this.header.memo);
                    writer.Write(this.header.xyzCustAxisStrings);
                    writer.Write(this.header.byteOffsettoLogBlock);
                    writer.Write(this.header.fileModificationFlag);
                    writer.Write(this.header.processingCode);
                    writer.Write(this.header.calibLvl);
                    writer.Write(this.header.subMethodSampleInjectionNumber);
                    writer.Write(this.header.floatingDataMultiplerConcentrationFactor);
                    writer.Write(this.header.methodFile);
                    writer.Write(this.header.ZsubFileIncForEvenZMultifiles);
                    writer.Write(this.header.numWPlanes);
                    writer.Write(this.header.WplaneInc);
                    writer.Write(this.header.WaxisUnitsCode);
                    writer.Write(this.header.reserved);

                    //write X 
                    for (int n = 0; n < data[0].dp.Count; n++)
                    {
                        writer.Write(data[0].dp[n].x);
                    }
                    //write subheader
                    writer.Write(data[0].subheader.subFileFlags);
                    writer.Write(data[0].subheader.exponentYVal);
                    writer.Write(data[0].subheader.subFileIndex);
                    writer.Write(data[0].subheader.subFileStartZ);
                    writer.Write(data[0].subheader.subFileEndZ);
                    writer.Write(data[0].subheader.subFileNoise);
                    writer.Write(data[0].subheader.numPoints);
                    writer.Write(data[0].subheader.numCoAddedScans);
                    writer.Write(data[0].subheader.WaxixVal);
                    writer.Write(data[0].subheader.reserved);

                    //write Y 
                    for (int n = 0; n < this.datablock[0].dp.Count; n++)
                    {
                        writer.Write(this.datablock[0].dp[n].y);
                    }
                }

                return 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

            return 1;
        }

        public int exportXYYFile(string filepath, List<datablock_t> data, int datablockCnt)
        {
            try
            {
                //write binary data
                using (BinaryWriter writer = new BinaryWriter(File.Open(SystemPath.GetLogPath + "\\" + DateTime.Now.ToString("yyyy-MMM") + "\\" + filepath + ".spc", FileMode.Create, FileAccess.ReadWrite)))
                {
                    //write main header
                    writer.Write(this.header.fileType);
                    writer.Write(this.header.fileVersion);
                    writer.Write(this.header.typeCode);
                    writer.Write(this.header.exponentYVal);
                    writer.Write(this.header.numPoints);
                    writer.Write(this.header.firstX);
                    writer.Write(this.header.lastX);
                    writer.Write(this.header.numSubFiles);
                    writer.Write(this.header.XunitsTypeCode);
                    writer.Write(this.header.YunitsTypeCode);
                    writer.Write(this.header.ZunitsTypeCode);
                    writer.Write(this.header.postingDisposition);
                    writer.Write(this.header.compressedDate);
                    writer.Write(this.header.resolutionDesc);
                    writer.Write(this.header.sourceIntrumentDes);
                    writer.Write(this.header.peakPointNumberForInerferograms);
                    for (int n = 0; n < this.header.spare.Length; n++)
                        writer.Write(this.header.spare[n]);
                    writer.Write(this.header.memo);
                    writer.Write(this.header.xyzCustAxisStrings);
                    writer.Write(this.header.byteOffsettoLogBlock);
                    writer.Write(this.header.fileModificationFlag);
                    writer.Write(this.header.processingCode);
                    writer.Write(this.header.calibLvl);
                    writer.Write(this.header.subMethodSampleInjectionNumber);
                    writer.Write(this.header.floatingDataMultiplerConcentrationFactor);
                    writer.Write(this.header.methodFile);
                    writer.Write(this.header.ZsubFileIncForEvenZMultifiles);
                    writer.Write(this.header.numWPlanes);
                    writer.Write(this.header.WplaneInc);
                    writer.Write(this.header.WaxisUnitsCode);
                    writer.Write(this.header.reserved);

                    //write X 
                    for (int n = 0; n < data[0].dp.Count; n++)
                    {
                        writer.Write(data[0].dp[n].x);
                    }

                    for (int p = 0; p < datablockCnt; p++)
                    {
                        //write subheader
                        writer.Write(data[p].subheader.subFileFlags);
                        writer.Write(data[p].subheader.exponentYVal);
                        writer.Write(data[p].subheader.subFileIndex);
                        writer.Write(data[p].subheader.subFileStartZ);
                        writer.Write(data[p].subheader.subFileEndZ);
                        writer.Write(data[p].subheader.subFileNoise);
                        writer.Write(data[p].subheader.numPoints);
                        writer.Write(data[p].subheader.numCoAddedScans);
                        writer.Write(data[p].subheader.WaxixVal);
                        writer.Write(data[p].subheader.reserved);

                        //write Y 
                        for (int n = 0; n < this.datablock[p].dp.Count; n++)
                        {
                            writer.Write(this.datablock[p].dp[n].y);
                        }
                    }

                }

                return 0;
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

            return 1;
        }
        #endregion
    }
}
