# TeddyBench Linux desktop installation

After extracting a TeddyBench Linux package, install it with:

```bash
cd TeddyBench-Linux
sudo ./install-linux.sh
```

This installs:

- The application to `/opt/teddybench`
- The application icon from `dog.ico`
- A desktop menu entry named **TeddyBench**

Then start TeddyBench from the Linux Mint application menu.

To remove it:

```bash
sudo /opt/teddybench/uninstall-linux.sh
```

If no PNG icon can be generated, the desktop entry may fall back to the system generic audio icon depending on the desktop environment.
