# SNP Viewer

A replacement to Microsoft's Snapshot Viewer application. 

The viewer works by converting the Snapshot file into a PDF using a library created by Stephen Lebans. Chromium's PDFium is then used to display document.

A Snapshot repair class by Andrew McCarthy is used to repair Snapshot files created by Access 2007 onwards.

The command line argument /attrib can be used to associate SNP files with the application.

This project needs to be compiled in 32bit to allow for compatibility with Stephen Lebans's library.

<p align="center">
  <img width="460" height="300" src="https://i.imgur.com/MoKd6pW.png">

</p>
