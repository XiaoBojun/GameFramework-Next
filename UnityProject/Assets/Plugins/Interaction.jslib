var mylib={
	SendUnityMessageToVue:function (key,value) {
        HandleUnityMessageToVue(Pointer_stringify(key),Pointer_stringify(value));
    },
}
mergeInto(LibraryManager.library, mylib);