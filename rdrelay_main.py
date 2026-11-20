#!/usr/bin/env python3
"""RDRelay — run directly: ./rdrelay.py or python3 rdrelay.py"""
import sys, os
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
from rdrelay.__main__ import main
main()
