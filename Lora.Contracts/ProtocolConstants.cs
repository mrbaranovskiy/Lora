using System;
using System.Runtime.InteropServices;

namespace MCP.DeviceDiscovery.Contracts;

public static class ProtocolConstants
{
    public const byte SERIALCMD_PING = 0x1;
}

public static class ResetDeviceConstants
{
    public const byte  RESET_DEVICE_DATA_00 = 200 ;
    public const byte  RESET_DEVICE_DATA_01 = 5 ;
    public const byte  RESET_DEVICE_DATA_02 = 173 ;
    public const byte  RESET_DEVICE_DATA_03 = 96 ;
    public const byte  RESET_DEVICE_DATA_04 = 210 ;
    public const byte  RESET_DEVICE_DATA_05 = 13 ;
    public const byte  RESET_DEVICE_DATA_06 = 66 ;
    public const byte  RESET_DEVICE_DATA_07 = 12 ;
    public const byte  RESET_DEVICE_DATA_08 = 127 ;
    public const byte  RESET_DEVICE_DATA_09 = 42 ;
}

public enum SerialCommand
{
    SERIALCMD_UNKNOWN = 0,
    SERIALCMD_PING = 1,
    SERIALCMD_ENUM_INFO = 2,
    SERIALCMD_GET_VERSION = 3,

    SERIALCMD_BOOT_ENTER = 4,
    SERIALCMD_BOOT_QUIT = 5,
    SERIALCMD_FLASH_ERASE = 6,
    SERIALCMD_FLASH_WRITE = 7,
    SERIALCMD_FLASH_READ = 8,
    SERIALCMD_SET_VERSION = 9,
    SERIALCMD_LAST_FIXED = 10,
    SERIALCMD_ADAPT_OW_DISABLE = 11,
    SERIALCMD_ADAPT_OW_ENABLE = 12,
    SERIALCMD_ADAPT_RESET = 13,
    SERIALCMD_ADAPT_LOADCELL_DATA = 14,
    SERIALCMD_ADAPT_LOADCELL_START_STREAM = 15,
    SERIALCMD_ADAPT_LOADCELL_STOP_STREAM = 16,
    SERIALCMD_ADAPT_EGIA_RELOAD_SWITCH_DATA = 17,
    SERIALCMD_ADAPT_EGIA_RELOAD_SWITCH_START_EVENTS = 18,
    SERIALCMD_ADAPT_EGIA_RELOAD_SWITCH_STOP_EVENTS = 19,
    SERIALCMD_ADAPT_WRITE_PGA308_REGISTER = 20,
    SERIALCMD_ADAPT_READ_PGA308_REGISTER = 21,
    SERIALCMD_ADAPT_EE_WRITE_RECOVERY_ITEM = 22,
    SERIALCMD_ENABLE_FLASH_COMMANDS = 23,
    SERIALCMD_ADAPT_EE_READ_RECOVERY_ITEM = 24,
    SERIALCMD_ASSERT_INFO = 25,
    SERIALCMD_DISPLAY_PROMPT = 26,
    SERIALCMD_DEBUG_STR = 27,
    SERIALCMD_HARDWARE_VERSION = 28,

    SERIALCMD_SERIAL_BUFFER_COUNTS = 29,
    SERIALCMD_LOG_TEXT = 30,
    SERIALCMD_NEW_ASSERT = 31,
    SERIALCMD_STREAMING_VAR_COUNT = 32,
    SERIALCMD_STREAMING_VAR_INFO = 33,

    SERIALCMD_CLEAR_STREAMING_LIST = 34,
    SERIALCMD_ADD_STREAMING_VAR = 35,
    SERIALCMD_REMOVE_STREAMING_VAR = 36,
    SERIALCMD_START_STREAMING = 37,
    SERIALCMD_STOP_STREAMING = 38,
    SERIALCMD_STREAMING_RATE = 39,
    SERIALCMD_STREAMING_DATA = 40,
    SERIALCMD_CHANGEABLE_VAR_COUNT = 41,
    SERIALCMD_CHANGEABLE_VAR_INFO = 42,

    SERIALCMD_CHANGEABLE_VAR_VALUE = 43,
    SERIALCMD_CHANGEABLE_VAR_UPDATE = 44,
    SERIALCMD_STATUS_VAR_COUNT = 45,
    SERIALCMD_STATUS_VAR_INFO = 46,
    SERIALCMD_STATUS_RATE = 47,
    SERIALCMD_STATUS_DATA = 48,
    SERIALCMD_STATUS_START = 49,
    SERIALCMD_STATUS_STOP = 50,
    SERIALCMD_DOPEN = 51,
    SERIALCMD_DCLOSE = 52,

    // SERIALCMD_FOPEN = 53,
    // SERIALCMD_FCLOSE = 54,
    SERIALCMD_NEXT_FILE_NAME = 55,
    SERIALCMD_CREATE_DIRECTORY = 56,
    SERIALCMD_CREATE_FILE = 57,
    SERIALCMD_FORMAT_FILESYSTEM = 58,
    SERIALCMD_DELETE_DIRECTORY = 59,
    SERIALCMD_DELETE_FILE = 60,
    SERIALCMD_RENAME_FILE = 61,
    SERIALCMD_SET_FILE_NAME = 62,
    SERIALCMD_GET_FILE_DATA = 63,
    SERIALCMD_SET_FILE_DATA = 64,
    SERIALCMD_GET_FILE_ATTRIB = 65,

    // SERIALCMD_GET_RTC = 66,
    // SERIALCMD_SET_RTC = 67,
    SERIALCMD_ONEWIRE_SEARCH_ALL_SLAVES = 68,
    SERIALCMD_ONEWIRE_GET_CONNECTED = 69,

    // SERIALCMD_ONEWIRE_GET_ADDRESS = 70,
    // SERIALCMD_ONEWIRE_GET_STATUS = 71,
    SERIALCMD_ONEWIRE_WRITE_MEMORY = 72,
    SERIALCMD_ONEWIRE_READ_MEMORY = 73,
    SERIALCMD_ONEWIRE_GET_ID = 74,

    // SERIALCMD_ONEWIRE_SET_ID = 75,
    // SERIALCMD_ONEWIRE_CLEAR_ALL_ID = 76,
    //SERIALCMD_ONEWIRE_DISABLE = 77,
    // SERIALCMD_ONEWIRE_UPLOAD_FAKE = 78,
    // SERIALCMD_RUN_MOTOR = 79,
    // SERIALCMD_COMM_TEST_SETUP = 80,
    // SERIALCMD_COMM_TEST_PACKET = 81,
    // SERIALCMD_BLOB_DATA_SETUP = 82,
    // SERIALCMD_BLOB_DATA_PACKET = 83,
    // SERIALCMD_BLOB_DATA_VALIDATE = 84,
    // SERIALCMD_FPGA_PGM_SETUP = 85,
    // SERIALCMD_FPGA_PGM_ENTER_WRITE_MODE = 86,
    // SERIALCMD_FPGA_PGM_PACKET = 87,
    // SERIALCMD_FPGA_PGM_VALIDATE = 88,
    // SERIALCMD_ERASE_HANDLE_TIMESTAMP = 89,
    // SERIALCMD_ERASE_HANDLE_BL_TIMESTAMP = 90,
    // SERIALCMD_ERASE_JED_TIMESTAMP = 91,
    // SERIALCMD_SET_JED_TIMESTAMP = 92,
    // SERIALCMD_GET_JED_TIMESTAMP = 93,
    // SERIALCMD_ACTIVE_TIMESTAMPS = 94,
    // SERIALCMD_DEVICE_PROPERTIES = 95,
    // SERIALCMD_FAT_READENTRY = 96,
    // SERIALCMD_SECTOR_READ = 97,
    // SERIALCMD_SECTOR_WRITE = 98,
    // SERIALCMD_WIFI_COMMAND = 99,
    // SERIALCMD_KVF_DESCRIPTION = 100,
    // SERIALCMD_TASK_LIST = 101,
    // SERIALCMD_TASK_NAME = 102,
    // SERIALCMD_TASK_STATS = 103,
    // SERIALCMD_READ_BATTERY_DATA = 104,
    SERIALCMD_BATTERY_COMMAND = 105,
    // SERIALCMD_BATTERY_SIMULATOR_DATA = 106,
    // SERIALCMD_PROFILER_TYPE_COUNT = 107,
    // SERIALCMD_PROFILER_TYPE_INFO = 108,
    SERIALCMD_PROFILER_HISTORY_START = 109,
    SERIALCMD_PROFILER_HISTORY_STOP = 110,
    SERIALCMD_PROFILER_HISTORY_DATA = 111,
    SERIALCMD_SIGNAL_TYPE_COUNT = 112,
    SERIALCMD_SIGNAL_TYPE_INFO = 113,

    //SERIALCMD_SIGNAL_DATA = 114,
    // SERIALCMD_OS_LOWEST_PRIORITY = 115,
    SERIALCMD_GET_SERIALNUM = 116,
    SERIALCMD_STRAINGAUGE = 117,
    //SERIALCMD_EMBED_VARS_INFO = 118,
    // SERIALCMD_EMBED_VARS_VALUES = 119,
    // SERIALCMD_TEST_CMD = 120,
    // SERIALCMD_GET_PARAMETERS = 121,
    SERIALCMD_RESET_DEVICE = 122,
    // SERIALCMD_ACCEL_SETTING = 123,
    // SERIALCMD_COUNTRY_CODE = 124,
    // SERIALCMD_GET_OPEN_FILE_DATA = 125,
    SERIALCMD_PASSWORD = 125

    // SERIALCMD_COUNT = 126

}

public enum ResetDeviceResponse : byte
{
    Success = 0,
    NoResponse,
    AdapterIsAttached 
}

public enum ErrorCode
{
    None, 
    TransportFailed,
    InternalError,
}
public enum FS_ERR
{
    FS_ERR_NONE = 0,
    FS_ERR_INVALID_ARG = 10, // Invalid argument. 
    FS_ERR_INVALID_CFG = 11, // Invalid configuration. 
    FS_ERR_INVALID_CHKSUM = 12, // Invalid checksum. 
    FS_ERR_INVALID_LEN = 13, // Invalid length. 
    FS_ERR_INVALID_TIME = 14, // Invalid date/time. 
    FS_ERR_INVALID_TIMESTAMP = 15, // Invalid timestamp. 
    FS_ERR_INVALID_TYPE = 16, // Invalid object type. 
    FS_ERR_MEM_ALLOC = 17, // Mem could not be alloc'd. 
    FS_ERR_NULL_ARG = 18, // Arg(s) passed NULL val(s). 
    FS_ERR_NULL_PTR = 19, // Ptr arg(s) passed NULL ptr(s). 
    FS_ERR_OS = 20, // OS err. 
    FS_ERR_OVF = 21, // Value too large to be stored in type. 
    FS_ERR_EOF = 22, // EOF reached. 
    FS_ERR_WORKING_DIR_NONE_AVAIL = 30, // No working dir avail. 
    FS_ERR_WORKING_DIR_INVALID = 31, // Working dir invalid. 
    //  BUFFER ERROR CODES 
    FS_ERR_BUF_NONE_AVAIL = 100, // No buf avail. 
    //  CACHE ERROR CODES 
    FS_ERR_CACHE_INVALID_MODE = 200, // Mode specified invalid. 
    FS_ERR_CACHE_INVALID_SEC_TYPE = 201, // Sector type specified invalid. 
    FS_ERR_CACHE_TOO_SMALL = 202, // Cache specified too small. 
    //  DEVICE ERROR CODES 
    FS_ERR_DEV = 300, // Device access error. 
    FS_ERR_DEV_ALREADY_OPEN = 301, // Device already open. 
    FS_ERR_DEV_CHNGD = 302, // Device has changed. 
    FS_ERR_DEV_FIXED = 303, // Device is fixed (cannot be closed). 
    FS_ERR_DEV_FULL = 304, // Device is full (no space could be allocated). 
    FS_ERR_DEV_INVALID = 310, // Invalid device. 
    FS_ERR_DEV_INVALID_CFG = 311, // Invalid dev cfg. 
    FS_ERR_DEV_INVALID_ECC = 312, // Invalid ECC. 
    FS_ERR_DEV_INVALID_IO_CTRL = 313, // I/O control invalid. 
    FS_ERR_DEV_INVALID_LOW_FMT = 314, // Low format invalid. 
    FS_ERR_DEV_INVALID_LOW_PARAMS = 315, // Invalid low-level device parameters. 
    FS_ERR_DEV_INVALID_MARK = 316, // Invalid mark. 
    FS_ERR_DEV_INVALID_NAME = 317, // Invalid device name. 
    FS_ERR_DEV_INVALID_OP = 318, // Invalid operation. 

    FS_ERR_DEV_INVALID_SEC_NBR = 319, // Invalid device sec nbr. 

    FS_ERR_DEV_INVALID_SEC_SIZE = 320, // Invalid device sec size. 

    FS_ERR_DEV_INVALID_SIZE = 321, // Invalid device size. 

    FS_ERR_DEV_INVALID_UNIT_NBR = 322, // Invalid device unit nbr. 

    FS_ERR_DEV_IO = 323, // Device I/O error. 

    FS_ERR_DEV_NONE_AVAIL = 324, // No device avail. 

    FS_ERR_DEV_NOT_OPEN = 325, // Device not open. 

    FS_ERR_DEV_NOT_PRESENT = 326, // Device not present. 

    FS_ERR_DEV_TIMEOUT = 327, // Device timeout. 

    FS_ERR_DEV_UNIT_NONE_AVAIL = 328, // No unit avail. 

    FS_ERR_DEV_UNIT_ALREADY_EXIST = 329, // Unit already exists. 

    FS_ERR_DEV_UNKNOWN = 330, // Unknown. 

    FS_ERR_DEV_VOL_OPEN = 331, // Vol open on dev. 

    FS_ERR_DEV_INCOMPATIBLE_LOW_PARAMS = 332, // Incompatible low-level device parameters. 

    FS_ERR_DEV_INVALID_METADATA = 333, // Device driver metadata is invalid. 

    FS_ERR_DEV_OP_ABORTED = 334, // Operation aborted. 

    FS_ERR_DEV_CORRUPT_LOW_FMT = 335, // Corrupted low-level fmt. 

    FS_ERR_DEV_INVALID_SEC_DATA = 336, // Retrieved sec data is invalid. 

    FS_ERR_DEV_WR_PROT = 337, // Device is write protected. 

    FS_ERR_DEV_OP_FAILED = 338, // Operation failed. 

    FS_ERR_DEV_NAND_NO_AVAIL_BLK = 350, // No blk avail. 

    FS_ERR_DEV_NAND_NO_SUCH_SEC = 351, // This sector is not available. 

    FS_ERR_DEV_NAND_ECC_NOT_SUPPORTED = 352, // The needed ECC scheme is not supported. 

    FS_ERR_DEV_NAND_ONFI_EXT_PARAM_PAGE = 362, // NAND device extended parameter page must be read. 


    //  DEVICE DRIVER ERROR CODES 

    FS_ERR_DEV_DRV_ALREADY_ADDED = 400, // Dev drv already added. 

    FS_ERR_DEV_DRV_INVALID_NAME = 401, // Invalid dev drv name. 

    FS_ERR_DEV_DRV_NONE_AVAIL = 402, // No driver available. 


    //  DIRECTORY ERROR CODES 

    FS_ERR_DIR_ALREADY_OPEN = 500, // Directory already open. 

    FS_ERR_DIR_DIS = 501, // Directory module disabled. 

    FS_ERR_DIR_FULL = 502, // Directory is full. 

    FS_ERR_DIR_NONE_AVAIL = 503, // No directory avail. 

    FS_ERR_DIR_NOT_OPEN = 504, // Directory not open. 


    //  ECC ERROR CODES 

    FS_ERR_ECC_CORR = 600, // Correctable ECC error. 

    FS_ERR_ECC_CRITICAL_CORR = 601, // Critical correctable ECC error (should refresh data). 

    FS_ERR_ECC_UNCORR = 602, // Uncorrectable ECC error. 


    //  ENTRY ERROR CODES 

    FS_ERR_ENTRIES_SAME = 700, // Paths specify same file system entry. 

    FS_ERR_ENTRIES_TYPE_DIFF = 701, // Paths do not both specify files OR directories. 

    FS_ERR_ENTRIES_VOLS_DIFF = 702, // Paths specify file system entries on different vols. 

    FS_ERR_ENTRY_CORRUPT = 703, // File system entry is corrupt. 

    FS_ERR_ENTRY_EXISTS = 704, // File system entry exists. 

    FS_ERR_ENTRY_INVALID = 705, // File system entry invalid. 

    FS_ERR_ENTRY_NOT_DIR = 706, // File system entry NOT a directory. 

    FS_ERR_ENTRY_NOT_EMPTY = 707, // File system entry NOT empty. 

    FS_ERR_ENTRY_NOT_FILE = 708, // File system entry NOT a file. 

    FS_ERR_ENTRY_NOT_FOUND = 709, // File system entry NOT found. 

    FS_ERR_ENTRY_PARENT_NOT_FOUND = 710, // Entry parent NOT found. 

    FS_ERR_ENTRY_PARENT_NOT_DIR = 711, // Entry parent NOT a directory. 

    FS_ERR_ENTRY_RD_ONLY = 712, // File system entry marked read-only. 

    FS_ERR_ENTRY_ROOT_DIR = 713, // File system entry is a root directory. 

    FS_ERR_ENTRY_TYPE_INVALID = 714, // File system entry type is invalid. 

    FS_ERR_ENTRY_OPEN = 715, // Operation not allowed on already open entry 

    FS_ERR_ENTRY_CLUS = 716, // No clus allocated to a directory entry 


    //  FILE ERROR CODES 

    FS_ERR_FILE_ALREADY_OPEN = 800, // File already open. 

    FS_ERR_FILE_BUF_ALREADY_ASSIGNED = 801, // Buf already assigned. 

    FS_ERR_FILE_ERR = 802, // Error indicator set on file. 

    FS_ERR_FILE_INVALID_ACCESS_MODE = 803, // Access mode is specified invalid. 

    FS_ERR_FILE_INVALID_ATTRIB = 804, // Attributes are specified invalid. 

    FS_ERR_FILE_INVALID_BUF_MODE = 805, // Buf mode is specified invalid or unknown. 

    FS_ERR_FILE_INVALID_BUF_SIZE = 806, // Buf size is specified invalid. 

    FS_ERR_FILE_INVALID_DATE_TIME = 807, // Date/time is specified invalid. 

    FS_ERR_FILE_INVALID_DATE_TIME_TYPE = 808, // Date/time type flag is specified invalid. 

    FS_ERR_FILE_INVALID_NAME = 809, // Name is specified invalid. 

    FS_ERR_FILE_INVALID_ORIGIN = 810, // Origin is specified invalid or unknown. 

    FS_ERR_FILE_INVALID_OFFSET = 811, // Offset is specified invalid. 

    FS_ERR_FILE_INVALID_FILES = 812, // Invalid file arguments. 

    FS_ERR_FILE_INVALID_OP = 813, // File operation invalid. 

    FS_ERR_FILE_INVALID_OP_SEQ = 814, // File operation sequence invalid. 

    FS_ERR_FILE_INVALID_POS = 815, // File position invalid. 

    FS_ERR_FILE_LOCKED = 816, // File locked. 

    FS_ERR_FILE_NONE_AVAIL = 817, // No file available. 

    FS_ERR_FILE_NOT_OPEN = 818, // File NOT open. 

    FS_ERR_FILE_NOT_LOCKED = 819, // File NOT locked. 

    FS_ERR_FILE_OVF = 820, // File size overflowed max file size. 

    FS_ERR_FILE_OVF_OFFSET = 821, // File offset overflowed max file offset. 


    //  NAME ERROR CODES 

    FS_ERR_NAME_BASE_TOO_LONG = 900, // Base name too long. 

    FS_ERR_NAME_EMPTY = 901, // Name empty. 

    FS_ERR_NAME_EXT_TOO_LONG = 902, // Extension too long. 

    FS_ERR_NAME_INVALID = 903, // Invalid file name or path. 

    FS_ERR_NAME_MIXED_CASE = 904, // Name is mixed case. 

    FS_ERR_NAME_NULL = 905, // Name ptr arg(s) passed NULL ptr(s). 

    FS_ERR_NAME_PATH_TOO_LONG = 906, // Entry path is too long. 

    FS_ERR_NAME_BUF_TOO_SHORT = 907, // Buffer for name is too short. 

    FS_ERR_NAME_TOO_LONG = 908, // Full name is too long. 


    //  PARTITION ERROR CODES 

    FS_ERR_PARTITION_INVALID = 1001, // Partition invalid. 

    FS_ERR_PARTITION_INVALID_NBR = 1002, // Partition nbr specified invalid. 

    FS_ERR_PARTITION_INVALID_SIG = 1003, // Partition sig invalid. 

    FS_ERR_PARTITION_INVALID_SIZE = 1004, // Partition size invalid. 

    FS_ERR_PARTITION_MAX = 1005, // Max nbr partitions have been created in MBR. 

    FS_ERR_PARTITION_NOT_FINAL = 1006, // Prev partition is not final partition. 

    FS_ERR_PARTITION_NOT_FOUND = 1007, // Partition NOT found. 

    FS_ERR_PARTITION_ZERO = 1008, // Partition zero. 


    //  POOLS ERROR CODE 

    FS_ERR_POOL_EMPTY = 1100, // Pool is empty. 

    FS_ERR_POOL_FULL = 1101, // Pool is full. 

    FS_ERR_POOL_INVALID_BLK_ADDR = 1102, // Block not found in used pool pointers. 

    FS_ERR_POOL_INVALID_BLK_IN_POOL = 1103, // Block found in free pool pointers. 

    FS_ERR_POOL_INVALID_BLK_IX = 1104, // Block index invalid. 

    FS_ERR_POOL_INVALID_BLK_NBR = 1105, // Number blocks specified invalid. 

    FS_ERR_POOL_INVALID_BLK_SIZE = 1106, // Block size specified invalid. 


    //  FILE SYSTEM ERROR CODES 

    FS_ERR_SYS_TYPE_NOT_SUPPORTED = 1301, // File sys type not supported. 

    FS_ERR_SYS_INVALID_SIG = 1309, // Sec has invalid OR illegal sig. 

    FS_ERR_SYS_DIR_ENTRY_PLACE = 1330, // Dir entry could not be placed. 

    FS_ERR_SYS_DIR_ENTRY_NOT_FOUND = 1331, // Dir entry not found. 

    FS_ERR_SYS_DIR_ENTRY_NOT_FOUND_YET = 1332, // Dir entry not found (yet). 

    FS_ERR_SYS_SEC_NOT_FOUND = 1333, // Sec not found. 

    FS_ERR_SYS_CLUS_CHAIN_END = 1334, // Cluster chain ended. 

    FS_ERR_SYS_CLUS_CHAIN_END_EARLY = 1335, // Cluster chain ended before number clusters traversed. 

    FS_ERR_SYS_CLUS_INVALID = 1336, // Cluster invalid. 

    FS_ERR_SYS_CLUS_NOT_AVAIL = 1337, // Cluster not avail. 

    FS_ERR_SYS_SFN_NOT_AVAIL = 1338, // SFN is not avail. 

    FS_ERR_SYS_LFN_ORPHANED = 1339, // LFN entry orphaned. 


    //  VOLUME ERROR CODES 

    FS_ERR_VOL_INVALID_NAME = 1400, // Invalid volume name. 

    FS_ERR_VOL_INVALID_SIZE = 1401, // Invalid volume size. 

    FS_ERR_VOL_INVALID_SEC_SIZE = 1403, // Invalid volume sector size. 

    FS_ERR_VOL_INVALID_CLUS_SIZE = 1404, // Invalid volume cluster size. 

    FS_ERR_VOL_INVALID_OP = 1405, // Volume operation invalid. 

    FS_ERR_VOL_INVALID_SEC_NBR = 1406, // Invalid volume sector number. 

    FS_ERR_VOL_INVALID_SYS = 1407, // Invalid file system on volume. 

    FS_ERR_VOL_NO_CACHE = 1408, // No cache assigned to volume. 

    FS_ERR_VOL_NONE_AVAIL = 1410, // No vol  avail. 

    FS_ERR_VOL_NONE_EXIST = 1411, // No vols exist. 

    FS_ERR_VOL_NOT_OPEN = 1413, // Vol NOT open. 

    FS_ERR_VOL_NOT_MOUNTED = 1414, // Vol NOT mounted. 

    FS_ERR_VOL_ALREADY_OPEN = 1415, // Vol already open. 

    FS_ERR_VOL_FILES_OPEN = 1416, // Files open on vol. 

    FS_ERR_VOL_DIRS_OPEN = 1417, // Dirs open on vol. 

    FS_ERR_VOL_JOURNAL_ALREADY_OPEN = 1420, // Journal already open. 

    FS_ERR_VOL_JOURNAL_CFG_CHNGD = 1421, // File system suite cfg changed since log created. 

    FS_ERR_VOL_JOURNAL_FILE_INVALID = 1422, // Journal file invalid. 

    FS_ERR_VOL_JOURNAL_FULL = 1423, // Journal full. 

    FS_ERR_VOL_JOURNAL_LOG_INVALID_ARG = 1424, // Invalid arg read from journal log. 

    FS_ERR_VOL_JOURNAL_LOG_INCOMPLETE = 1425, // Log not completely entered in journal. 

    FS_ERR_VOL_JOURNAL_LOG_NOT_PRESENT = 1426, // Log not present in journal. 

    FS_ERR_VOL_JOURNAL_NOT_OPEN = 1427, // Journal not open. 

    FS_ERR_VOL_JOURNAL_NOT_REPLAYING = 1428, // Journal not being replayed. 

    FS_ERR_VOL_JOURNAL_NOT_STARTED = 1429, // Journaling not started. 

    FS_ERR_VOL_JOURNAL_NOT_STOPPED = 1430, // Journaling not stopped. 

    FS_ERR_VOL_LABEL_INVALID = 1440, // Volume label is invalid. 

    FS_ERR_VOL_LABEL_NOT_FOUND = 1441, // Volume label was not found. 

    FS_ERR_VOL_LABEL_TOO_LONG = 1442, // Volume label is too long. 
    //  FILE SYSTEM-OS LAYER ERROR CODES 
    FS_ERR_OS_LOCK = 1501,
    FS_ERR_OS_LOCK_TIMEOUT = 1502,
    FS_ERR_OS_INIT = 1510,
    FS_ERR_OS_INIT_LOCK = 1511,
    FS_ERR_OS_INIT_LOCK_NAME = 1512,
}


#pragma warning disable CS8618


[StructLayout(LayoutKind.Sequential)]
public struct FsEntryInfo
{
    public uint Attrib; // Entry attributes.
    public uint Size; // File size in octets.
    public uint DateTimeCreate; // Date/time of creation.
    public uint DateAccess; // Date of last access.
    public uint DateTimeWr; // Date/time of last write.
    public uint BlkCnt; // Number of blocks allocated for file.
    public uint BlkSize; // Block size.
}

[Flags]
public enum FsEntryAttr
{
    FS_ENTRY_ATTRIB_NONE = 0x0,
    FS_ENTRY_ATTRIB_RD = 1 << 0,
    FS_ENTRY_ATTRIB_WR = 1 << 1,
    FS_ENTRY_ATTRIB_HIDDEN = 1 << 2,
    FS_ENTRY_ATTRIB_DIR = 1 << 6,
    FS_ENTRY_ATTRIB_DIR_ROOT = 1 << 7
}

public record FSEntry(string Path, TreeViewItemType FileType, bool IsEmpty);