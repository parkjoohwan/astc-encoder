# CMakeList.txt: astc_lib에 대한 CMake 프로젝트, 여기에 소스를 포함하고
# 프로젝트 특정 논리를 정의합니다.
#
cmake_minimum_required (VERSION 3.8)

# astc-encoder 프로젝트 include => cpp 프로젝트
include(ExternalProject)

# astc encoder include
# Add the external project and pull out the project directories we need

# The default build is a native build which supports the highest level of SIMD
# exposed by the compiler when using default compiler flags. Add a single
# SIMD enable to the CMAKE_CACHE_ARGS option to force something specific, but
# remember to change the link library in target_link_libraries() to match.
#
#  *  Add "-DISA_SSE2:String=ON" and link against "astcenc-sse2-static"
#  *  Add "-DISA_SSE41:String=ON" and link against "astcenc-sse4.1-static"
#  *  Add "-DISA_AVX2:String=ON" and link against "astcenc-avx2-static"
#  *  Add "-DISA_NEON:String=ON" and link against "astcenc-neon-static"
ExternalProject_Add(astc
    GIT_REPOSITORY https://github.com/ARM-software/astc-encoder
    GIT_TAG main
    CMAKE_CACHE_ARGS -DCLI:STRING=OFF -DCMAKE_BUILD_TYPE:STRING=${CMAKE_BUILD_TYPE} -DISA_SSE2:STRING=ON
    INSTALL_COMMAND "")

ExternalProject_Get_property(astc
    SOURCE_DIR)

ExternalProject_Get_property(astc
    BINARY_DIR)

        
add_library(astcenc_library SHARED astc_lib.cpp)
add_dependencies(astcenc_library astc)

# ... with astcencoder Source dir on the include path
target_include_directories(astcenc_library
    PRIVATE
        ${SOURCE_DIR}/Source)

# ... with astcencoder Binary dir on the library path and as a library dep
target_link_directories(astcenc_library
    PRIVATE
        ${BINARY_DIR}/Source)
        
#target_link_libraries(astcenc_library
#    PRIVATE
#        astcenc-native-static)
        
target_link_libraries(astcenc_library
    PRIVATE
        astcenc-sse2-static)

#set(CMAKE_CXX_FLAGS_DEBUG "/MTd")
#set(CMAKE_CXX_FLAGS_DEBUG "/MTd") 
        
add_library(astcenc_library2 SHARED "astc_lib_main.cpp" "astc_lib_help.cpp" "astc_lib_image.cpp" "astc_lib_image_external.cpp" "astc_lib_image_load_store.cpp" "astc_lib_platform_dependents.cpp" "astc_lib_error_metrics.cpp")
add_dependencies(astcenc_library2 astc)

# ... with astcencoder Source dir on the include path
target_include_directories(astcenc_library2
    PRIVATE
        ${SOURCE_DIR}/Source)

# ... with astcencoder Binary dir on the library path and as a library dep
target_link_directories(astcenc_library2
    PRIVATE
        ${BINARY_DIR}/Source)
        
#target_link_libraries(astcenc_library
#    PRIVATE
#        astcenc-native-static)
        
target_link_libraries(astcenc_library2
    PRIVATE
        astcenc-sse2-static)
