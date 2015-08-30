using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace baseLibrary.Generic
{
    internal static class SQLRepository
    {
        public static String MASTER_CONFIG = "MASTER_CONFIG";
        public static String REMOTE_DATA = "REMOTE_DATA";
        public static String LOCAL_DATA = "LOCAL_DATA";
        public static String JOIN_DATA = "JOIN_DATA";
        public static String FLICKR_ALBUMS = "FLICKR_ALBUMS";

        #region Static SQLs
        public static String _loadLocalDuplicatesSQL =
        @"select  album src,path2 dest, count(1) numPics from 
            (select l1.filename, l1.date_taken, l1.path album, l2.path path2 from local_data l1, local_data l2 where
            l1.filename = l2.filename and
            l1.date_taken = l2.date_taken and
            l1.path != l2.path
            order by l1.filename desc)
        group by album,path2;";

        public static String _loadLocalDuplicateImagesSQL =
        @"select l1.filename, l1.date_taken, l1.path album, l2.path path2 from local_data l1, local_data l2 where
            l1.filename = l2.filename and
            l1.date_taken = l2.date_taken and
            l1.path != l2.path and
            l1.path = '{0}' and
            l2.path = '{1}'
            order by l1.filename desc;";

        public static String _loadRemoteDuplicatesSQL =
            @"select  album src,path2 dest, count(1) numPics from 
                (select l1.TITLE, l1.date_taken, l1.ALBUM album, l2.ALBUM path2, l1.id from remote_data l1, remote_data l2 where
                l1.TITLE = l2.TITLE and
                l1.date_taken = l2.date_taken and
                l1.ALBUM != l2.ALBUM
                order by l1.ALBUM desc)
                group by album,path2;";

        public static String _loadRemoteDuplicateImagesSQL = 
            @"select l1.TITLE, l1.date_taken, l1.ALBUM album, l2.ALBUM path2, l1.ID from remote_data l1, remote_data l2 where
                l1.TITLE = l2.TITLE and
                l1.date_taken = l2.date_taken and
                l1.ALBUM != l2.ALBUM and
                l1.ALBUM = '{0}' and
                l2.ALBUM = '{1}'
                order by l1.TITLE desc;";


        public static String _createTable = "CREATE TABLE if not exists ";
        public static String _CreateMasterConfigTable = _createTable + SQLRepository.MASTER_CONFIG + "(CONFIG_TYPE TEXT, PARAM TEXT, VALUE TEXT);";
        public static String _CreateFlickrAlbumsTable = _createTable + SQLRepository.FLICKR_ALBUMS + 
            "(ID TEXT, NAME TEXT, DATE_CREATED TEXT, NUM_PICS INTEGER, ACTUAL_NUM_PICS INTEGER, NUM_VID INTEGER, ACTUAL_NUM_VID INTEGER, DESCRIPTION TEXT, SYNC_DATE TEXT);";
        public static String _CreateRemoteDataTable = _createTable + SQLRepository.REMOTE_DATA + "(TITLE TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, ALBUM TEXT, ID TEXT, PROCESSED TEXT, SYNC_DATE TEXT);";
        public static String _CreateLocalDataTable = _createTable + SQLRepository.LOCAL_DATA + "(FILENAME TEXT, DATE_TAKEN TEXT, DESCRIPTION TEXT, PATH TEXT, SIZE INTEGER, PROCESSED TEXT, SYNC_DATE TEXT);";
        public static String _CreateJoinDataTable = _createTable + SQLRepository.JOIN_DATA + "(NAME TEXT, DATE_TAKEN TEXT, FLICKER_PATH TEXT, LOCAL_PATH TEXT, COUNT INTEGER);";
        #endregion


    }
}
